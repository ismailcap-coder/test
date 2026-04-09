using System.Collections.Generic;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCitizens { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public decimal TotalFeesCollected { get; set; }
        public IList<RecentApplicationItem> RecentApplications { get; set; }
        public IList<StatusSummaryItem> StatusSummary { get; set; }

        public DashboardViewModel()
        {
            RecentApplications = new List<RecentApplicationItem>();
            StatusSummary = new List<StatusSummaryItem>();
        }
    }

    public class RecentApplicationItem
    {
        public string ApplicationNumber { get; set; }
        public string CitizenName { get; set; }
        public string ServiceName { get; set; }
        public string OfficeName { get; set; }
        public ApplicationStatus Status { get; set; }
        public decimal CalculatedFee { get; set; }
        public string SubmissionDate { get; set; }
    }

    public class StatusSummaryItem
    {
        public string StatusName { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}
