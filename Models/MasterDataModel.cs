using System;

namespace VMS.Models
{
    public class MasterDeptModel
    {
        public int DeptID { get; set; }
        public string DeptName { get; set; }
        public bool IsActive { get; set; }
    }

    public class MasterIDProofModel
    {
        public int IDProofID { get; set; }
        public string ProofName { get; set; }
        public bool IsActive { get; set; }
    }

    public class MasterHostModel
    {
        public int HostID { get; set; }
        public string HostName { get; set; }
        public int DeptID { get; set; }
        public string DeptName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public bool IsActive { get; set; }
    }

    public class MasterGateModel
    {
        public int GateID { get; set; }
        public string GateNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
