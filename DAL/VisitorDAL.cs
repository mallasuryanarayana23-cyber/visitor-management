using System;
using System.Data;
using Npgsql;
using VMS.Models;
using System.Collections.Generic;

namespace VMS.DAL
{
    public class VisitorDAL
    {
        public void RegisterVisitor(VisitorModel visitor, out int newVisitorId, out string newToken)
        {
            string query = @"
                INSERT INTO VMS_VISITORS (
                    VISIT_TOKEN, FULL_NAME, MOBILE, EMAIL, COMPANY_NAME, PURPOSE, 
                    HOST_ID, DEPT_ID, EXPECTED_DATETIME, IDPROOF_TYPE_ID, IDPROOF_NUMBER, REGISTERED_BY
                ) VALUES (
                    @p_TOKEN, @p_FULL_NAME, @p_MOBILE, @p_EMAIL, @p_COMPANY_NAME, @p_PURPOSE,
                    @p_HOST_ID, @p_DEPT_ID, @p_EXPECTED_DATETIME, @p_IDPROOF_TYPE_ID, @p_IDPROOF_NUMBER, @p_REGISTERED_BY
                ) RETURNING VISITOR_ID;";

            string dateStr = DateTime.Now.ToString("yyyyMMdd");
            string tokenPrefix = $"VMS-{dateStr}-";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@p_FULL_NAME", visitor.FullName),
                new NpgsqlParameter("@p_MOBILE", visitor.Mobile),
                new NpgsqlParameter("@p_EMAIL", (object)visitor.Email ?? DBNull.Value),
                new NpgsqlParameter("@p_COMPANY_NAME", (object)visitor.CompanyName ?? DBNull.Value),
                new NpgsqlParameter("@p_PURPOSE", visitor.Purpose),
                new NpgsqlParameter("@p_HOST_ID", visitor.HostID),
                new NpgsqlParameter("@p_DEPT_ID", visitor.DeptID),
                new NpgsqlParameter("@p_EXPECTED_DATETIME", visitor.ExpectedDateTime),
                new NpgsqlParameter("@p_IDPROOF_TYPE_ID", visitor.IDProofTypeID),
                new NpgsqlParameter("@p_IDPROOF_NUMBER", visitor.IDProofNumber),
                new NpgsqlParameter("@p_REGISTERED_BY", visitor.RegisteredBy)
            };

            using (NpgsqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                using (NpgsqlCommand seqCmd = new NpgsqlCommand("SELECT nextval('seq_vms_visitors')", conn))
                {
                    long nextVal = (long)seqCmd.ExecuteScalar();
                    newToken = tokenPrefix + nextVal.ToString("D4");
                }

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(parameters);
                    cmd.Parameters.AddWithValue("@p_TOKEN", newToken);
                    newVisitorId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public void CheckInVisitor(int visitorId, int gateId, int guardId)
        {
            string query = @"
                UPDATE VMS_VISITORS SET STATUS = 'Checked-In' WHERE VISITOR_ID = @p_VISITOR_ID;
                INSERT INTO VMS_CHECKIN_LOG (VISITOR_ID, GATE_ID, GUARD_ID, CHECKIN_TIME) 
                VALUES (@p_VISITOR_ID, @p_GATE_ID, @p_GUARD_ID, CURRENT_TIMESTAMP);";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@p_VISITOR_ID", visitorId),
                new NpgsqlParameter("@p_GATE_ID", gateId),
                new NpgsqlParameter("@p_GUARD_ID", guardId)
            };

            DBHelper.ExecuteNonQuery(query, parameters);
        }

        public void CheckOutVisitor(int visitorId)
        {
            string query = @"
                UPDATE VMS_VISITORS SET STATUS = 'Checked-Out' WHERE VISITOR_ID = @p_VISITOR_ID;
                WITH LatestCheckin AS (
                    SELECT LOG_ID FROM VMS_CHECKIN_LOG 
                    WHERE VISITOR_ID = @p_VISITOR_ID AND CHECKOUT_TIME IS NULL 
                    ORDER BY CHECKIN_TIME DESC LIMIT 1
                )
                UPDATE VMS_CHECKIN_LOG 
                SET CHECKOUT_TIME = CURRENT_TIMESTAMP, 
                    DURATION_MINUTES = EXTRACT(EPOCH FROM (CURRENT_TIMESTAMP - CHECKIN_TIME)) / 60
                WHERE LOG_ID = (SELECT LOG_ID FROM LatestCheckin);
            ";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@p_VISITOR_ID", visitorId)
            };

            DBHelper.ExecuteNonQuery(query, parameters);
        }

        public DashboardModel GetDashboardCounts()
        {
            using (NpgsqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                long todayExpected = (long)new NpgsqlCommand("SELECT COUNT(*) FROM VMS_VISITORS WHERE DATE(EXPECTED_DATETIME) = CURRENT_DATE", conn).ExecuteScalar();
                long checkedIn = (long)new NpgsqlCommand("SELECT COUNT(*) FROM VMS_VISITORS WHERE STATUS = 'Checked-In'", conn).ExecuteScalar();
                long checkedOut = (long)new NpgsqlCommand("SELECT COUNT(*) FROM VMS_VISITORS WHERE DATE(EXPECTED_DATETIME) = CURRENT_DATE AND STATUS = 'Checked-Out'", conn).ExecuteScalar();
                long pending = (long)new NpgsqlCommand("SELECT COUNT(*) FROM VMS_VISITORS WHERE STATUS = 'Pending'", conn).ExecuteScalar();
                long monthRegistered = (long)new NpgsqlCommand("SELECT COUNT(*) FROM VMS_VISITORS WHERE EXTRACT(MONTH FROM CREATED_DATE) = EXTRACT(MONTH FROM CURRENT_DATE) AND EXTRACT(YEAR FROM CREATED_DATE) = EXTRACT(YEAR FROM CURRENT_DATE)", conn).ExecuteScalar();

                return new DashboardModel
                {
                    TodayExpected = Convert.ToInt32(todayExpected),
                    CheckedIn = Convert.ToInt32(checkedIn),
                    CheckedOut = Convert.ToInt32(checkedOut),
                    PendingApprovals = Convert.ToInt32(pending),
                    TotalRegisteredThisMonth = Convert.ToInt32(monthRegistered)
                };
            }
        }

        public List<VisitorModel> GetVisitorReport(DateTime start, DateTime end, int deptId)
        {
            string query = @"
                SELECT * FROM VW_VISITOR_FULL_DETAIL 
                WHERE DATE(EXPECTED_DATETIME) >= @p_START_DATE 
                  AND DATE(EXPECTED_DATETIME) <= @p_END_DATE
                  AND (@p_DEPT_ID = 0 OR DEPT_ID = @p_DEPT_ID)";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@p_START_DATE", start.Date),
                new NpgsqlParameter("@p_END_DATE", end.Date),
                new NpgsqlParameter("@p_DEPT_ID", deptId)
            };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters);
            return MapToVisitorList(dt);
        }

        private List<VisitorModel> MapToVisitorList(DataTable dt)
        {
            List<VisitorModel> list = new List<VisitorModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new VisitorModel
                {
                    VisitorID = Convert.ToInt32(row["VISITOR_ID"]),
                    VisitToken = row["VISIT_TOKEN"].ToString(),
                    FullName = row["FULL_NAME"].ToString(),
                    Mobile = row["MOBILE"].ToString(),
                    CompanyName = row["COMPANY_NAME"].ToString(),
                    Purpose = row["PURPOSE"].ToString(),
                    ExpectedDateTime = Convert.ToDateTime(row["EXPECTED_DATETIME"]),
                    Status = row["STATUS"].ToString(),
                    PhotoUrl = row["PHOTO_URL"] != DBNull.Value ? row["PHOTO_URL"].ToString() : "",
                    IDProofUrl = row["IDPROOF_URL"] != DBNull.Value ? row["IDPROOF_URL"].ToString() : "",
                    HostName = row["HOST_NAME"] != DBNull.Value ? row["HOST_NAME"].ToString() : "",
                    DeptName = row["DEPT_NAME"] != DBNull.Value ? row["DEPT_NAME"].ToString() : ""
                });
            }
            return list;
        }
    }
}
