using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using VMS.Models;
using System.Collections.Generic;

namespace VMS.DAL
{
    public class VisitorDAL
    {
        public void RegisterVisitor(VisitorModel visitor, out int newVisitorId, out string newToken)
        {
            OracleParameter pNewVisitorId = new OracleParameter("p_OUT_VISITOR_ID", OracleDbType.Int32) { Direction = ParameterDirection.Output };
            OracleParameter pNewToken = new OracleParameter("p_OUT_TOKEN", OracleDbType.Varchar2, 50) { Direction = ParameterDirection.Output };

            OracleParameter[] parameters = {
                new OracleParameter("p_FULL_NAME", OracleDbType.Varchar2) { Value = visitor.FullName },
                new OracleParameter("p_MOBILE", OracleDbType.Varchar2) { Value = visitor.Mobile },
                new OracleParameter("p_EMAIL", OracleDbType.Varchar2) { Value = (object)visitor.Email ?? DBNull.Value },
                new OracleParameter("p_COMPANY_NAME", OracleDbType.Varchar2) { Value = (object)visitor.CompanyName ?? DBNull.Value },
                new OracleParameter("p_PURPOSE", OracleDbType.Varchar2) { Value = visitor.Purpose },
                new OracleParameter("p_HOST_ID", OracleDbType.Int32) { Value = visitor.HostID },
                new OracleParameter("p_DEPT_ID", OracleDbType.Int32) { Value = visitor.DeptID },
                new OracleParameter("p_EXPECTED_DATETIME", OracleDbType.TimeStamp) { Value = visitor.ExpectedDateTime },
                new OracleParameter("p_IDPROOF_TYPE_ID", OracleDbType.Int32) { Value = visitor.IDProofTypeID },
                new OracleParameter("p_IDPROOF_NUMBER", OracleDbType.Varchar2) { Value = visitor.IDProofNumber },
                new OracleParameter("p_REGISTERED_BY", OracleDbType.Int32) { Value = visitor.RegisteredBy },
                pNewVisitorId,
                pNewToken
            };

            DBHelper.ExecuteNonQueryWithOutParams("SP_REGISTER_VISITOR", parameters);

            newVisitorId = Convert.ToInt32(pNewVisitorId.Value.ToString());
            newToken = pNewToken.Value.ToString();
        }

        public void CheckInVisitor(int visitorId, int gateId, int guardId)
        {
            OracleParameter[] parameters = {
                new OracleParameter("p_VISITOR_ID", OracleDbType.Int32) { Value = visitorId },
                new OracleParameter("p_GATE_ID", OracleDbType.Int32) { Value = gateId },
                new OracleParameter("p_GUARD_ID", OracleDbType.Int32) { Value = guardId }
            };

            DBHelper.ExecuteNonQuery("SP_CHECKIN_VISITOR", parameters);
        }

        public void CheckOutVisitor(int visitorId)
        {
            OracleParameter[] parameters = {
                new OracleParameter("p_VISITOR_ID", OracleDbType.Int32) { Value = visitorId }
            };

            DBHelper.ExecuteNonQuery("SP_CHECKOUT_VISITOR", parameters);
        }

        public DashboardModel GetDashboardCounts()
        {
            OracleParameter pExpected = new OracleParameter("p_TODAY_EXPECTED", OracleDbType.Int32) { Direction = ParameterDirection.Output };
            OracleParameter pIn = new OracleParameter("p_CHECKED_IN", OracleDbType.Int32) { Direction = ParameterDirection.Output };
            OracleParameter pOut = new OracleParameter("p_CHECKED_OUT", OracleDbType.Int32) { Direction = ParameterDirection.Output };
            OracleParameter pPending = new OracleParameter("p_PENDING", OracleDbType.Int32) { Direction = ParameterDirection.Output };
            OracleParameter pMonth = new OracleParameter("p_MONTH_REGISTERED", OracleDbType.Int32) { Direction = ParameterDirection.Output };

            OracleParameter[] parameters = { pExpected, pIn, pOut, pPending, pMonth };

            DBHelper.ExecuteNonQueryWithOutParams("SP_GET_DASHBOARD_COUNTS", parameters);

            return new DashboardModel
            {
                TodayExpected = Convert.ToInt32(pExpected.Value.ToString()),
                CheckedIn = Convert.ToInt32(pIn.Value.ToString()),
                CheckedOut = Convert.ToInt32(pOut.Value.ToString()),
                PendingApprovals = Convert.ToInt32(pPending.Value.ToString()),
                TotalRegisteredThisMonth = Convert.ToInt32(pMonth.Value.ToString())
            };
        }

        public List<VisitorModel> GetVisitorReport(DateTime start, DateTime end, int deptId)
        {
            OracleParameter pCursor = new OracleParameter("p_RECORDSET", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
            
            OracleParameter[] parameters = {
                new OracleParameter("p_START_DATE", OracleDbType.Date) { Value = start },
                new OracleParameter("p_END_DATE", OracleDbType.Date) { Value = end },
                new OracleParameter("p_DEPT_ID", OracleDbType.Int32) { Value = deptId },
                pCursor
            };

            DataTable dt = DBHelper.ExecuteQuery("SP_GET_VISITOR_REPORT", parameters);
            return MapToVisitorList(dt);
        }

        // Helper Map Method
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
