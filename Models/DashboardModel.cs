using System;

namespace VMS.Models
{
    public class DashboardModel
    {
        public int TodayExpected { get; set; }
        public int CheckedIn { get; set; }
        public int CheckedOut { get; set; }
        public int PendingApprovals { get; set; }
        public int TotalRegisteredThisMonth { get; set; }
    }
}
