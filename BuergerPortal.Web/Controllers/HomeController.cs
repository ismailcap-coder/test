using System.Web.Mvc;
using BuergerPortal.Data;
using BuergerPortal.Domain.Enums;
using System.Linq;

namespace BuergerPortal.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly BuergerPortalContext _context;

        public HomeController()
        {
            _context = new BuergerPortalContext();
        }

        public ActionResult Index()
        {
            var dashboard = new ViewModels.DashboardViewModel
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

            // Recent applications
            var recentApps = _context.ServiceApplications
                .OrderByDescending(a => a.SubmissionDate)
                .Take(10)
                .ToList();

            foreach (var app in recentApps)
            {
                var citizen = _context.Citizens.Find(app.CitizenId);
                var serviceType = _context.ServiceTypes.Find(app.ServiceTypeId);
                var office = _context.PublicOffices.Find(app.OfficeId);
                dashboard.RecentApplications.Add(new ViewModels.RecentApplicationItem
                {
                    ApplicationNumber = app.ApplicationNumber,
                    CitizenName = citizen != null ? citizen.FullName : "Unknown",
                    ServiceName = serviceType != null ? serviceType.ServiceName : "Unknown",
                    OfficeName = office != null ? office.OfficeName : "Unknown",
                    Status = app.Status,
                    CalculatedFee = app.CalculatedFee,
                    SubmissionDate = app.SubmissionDate.HasValue ? app.SubmissionDate.Value.ToShortDateString() : "N/A"
                });
            }

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
                    dashboard.StatusSummary.Add(new ViewModels.StatusSummaryItem
                    {
                        StatusName = status.ToString(),
                        Count = count,
                        Percentage = (decimal)count / total * 100
                    });
                }
            }

            return View(dashboard);
        }

        public ActionResult About()
        {
            ViewData["Message"] = "BuergerPortal - Citizen Services Management for German Public Offices.";
            return View();
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
