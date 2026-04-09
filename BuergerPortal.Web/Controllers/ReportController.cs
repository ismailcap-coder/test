using System.Linq;
using System.Web.Mvc;
using BuergerPortal.Data;
using BuergerPortal.Domain.Enums;
using BuergerPortal.Web.ViewModels;

namespace BuergerPortal.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly BuergerPortalContext _context;

        public ReportController()
        {
            _context = new BuergerPortalContext();
        }

        public ActionResult Dashboard()
        {
            var dashboard = new DashboardViewModel
            {
                TotalCitizens = _context.Citizens.Count(),
                TotalApplications = _context.ServiceApplications.Count(),
                PendingApplications = _context.ServiceApplications.Count(a =>
                    a.Status == ApplicationStatus.Submitted || a.Status == ApplicationStatus.UnderReview),
                ApprovedApplications = _context.ServiceApplications.Count(a =>
                    a.Status == ApplicationStatus.Approved),
                RejectedApplications = _context.ServiceApplications.Count(a =>
                    a.Status == ApplicationStatus.Rejected),
                TotalFeesCollected = _context.ServiceApplications
                    .Where(a => a.Status == ApplicationStatus.Approved)
                    .Select(a => a.CalculatedFee)
                    .DefaultIfEmpty(0)
                    .Sum()
            };

            // Status summary
            int total = dashboard.TotalApplications;
            if (total > 0)
            {
                var statuses = new[] {
                    ApplicationStatus.Draft, ApplicationStatus.Submitted,
                    ApplicationStatus.UnderReview, ApplicationStatus.DocumentsRequested,
                    ApplicationStatus.Approved, ApplicationStatus.Rejected
                };
                foreach (var status in statuses)
                {
                    int count = _context.ServiceApplications.Count(a => a.Status == status);
                    dashboard.StatusSummary.Add(new StatusSummaryItem
                    {
                        StatusName = status.ToString(),
                        Count = count,
                        Percentage = (decimal)count / total * 100
                    });
                }
            }

            return View(dashboard);
        }

        public ActionResult ApplicationsByOffice()
        {
            var data = _context.PublicOffices
                .ToList()
                .Select(o => new
                {
                    OfficeName = o.OfficeName,
                    City = o.City,
                    Total = _context.ServiceApplications.Count(a => a.OfficeId == o.OfficeId),
                    Pending = _context.ServiceApplications.Count(a => a.OfficeId == o.OfficeId
                        && (a.Status == ApplicationStatus.Submitted || a.Status == ApplicationStatus.UnderReview)),
                    Approved = _context.ServiceApplications.Count(a => a.OfficeId == o.OfficeId
                        && a.Status == ApplicationStatus.Approved)
                })
                .ToList();

            return View(data);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
