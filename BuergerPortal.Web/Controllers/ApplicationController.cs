using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BuergerPortal.Business.Services;
using BuergerPortal.Business.Validators;
using BuergerPortal.Data;
using BuergerPortal.Data.Repositories;
using BuergerPortal.Domain.Enums;
using BuergerPortal.Web.ViewModels;

namespace BuergerPortal.Web.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly BuergerPortalContext _context;

        public ApplicationController(IApplicationService applicationService, BuergerPortalContext context)
        {
            _applicationService = applicationService;
            _context = context;
        }

        public IActionResult Index(ApplicationStatus? status)
        {
            var applications = status.HasValue
                ? _applicationService.GetApplicationsByStatus(status.Value)
                : _applicationService.GetAllApplications();

            var viewModels = new List<ApplicationViewModel>();

            foreach (var a in applications)
            {
                var vm = new ApplicationViewModel
                {
                    ApplicationId = a.ApplicationId,
                    ApplicationNumber = a.ApplicationNumber,
                    Status = a.Status,
                    SubmissionDate = a.SubmissionDate,
                    CalculatedFee = a.CalculatedFee,
                    IsExpressProcessing = a.IsExpressProcessing,
                    CitizenName = a.Citizen != null ? a.Citizen.FullName : "N/A",
                    ServiceTypeName = a.ServiceType != null ? a.ServiceType.ServiceName : "N/A",
                    OfficeName = a.Office != null ? a.Office.OfficeName : "N/A"
                };

                viewModels.Add(vm);
            }

            ViewData["CurrentStatus"] = status;
            return View(viewModels);
        }

        public IActionResult Details(int id)
        {
            var application = _applicationService.GetApplicationWithDetails(id);
            var viewModel = new ApplicationViewModel
            {
                ApplicationId = application.ApplicationId,
                ApplicationNumber = application.ApplicationNumber,
                CitizenId = application.CitizenId,
                ServiceTypeId = application.ServiceTypeId,
                OfficeId = application.OfficeId,
                Status = application.Status,
                SubmissionDate = application.SubmissionDate,
                ReviewDate = application.ReviewDate,
                CompletionDate = application.CompletionDate,
                DeadlineDate = application.DeadlineDate,
                IsExpressProcessing = application.IsExpressProcessing,
                CalculatedFee = application.CalculatedFee,
                Notes = application.Notes,
                RejectionReason = application.RejectionReason,
                CitizenName = application.Citizen != null ? application.Citizen.FullName : "N/A",
                ServiceTypeName = application.ServiceType != null ? application.ServiceType.ServiceName : "N/A",
                OfficeName = application.Office != null ? application.Office.OfficeName : "N/A"
            };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            var viewModel = new ApplicationViewModel();
            PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var application = _applicationService.CreateApplication(
                        viewModel.CitizenId,
                        viewModel.ServiceTypeId,
                        viewModel.OfficeId,
                        viewModel.IsExpressProcessing,
                        viewModel.Notes ?? string.Empty);
                    TempData["SuccessMessage"] = string.Format(
                        "Application {0} created successfully. Calculated fee: {1:C2}",
                        application.ApplicationNumber, application.CalculatedFee);
                    return RedirectToAction("Details", new { id = application.ApplicationId });
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(int id)
        {
            try
            {
                _applicationService.SubmitApplication(id, User.Identity?.Name ?? "System");
                TempData["SuccessMessage"] = "Application submitted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartReview(int id)
        {
            try
            {
                _applicationService.StartReview(id, User.Identity?.Name ?? "Reviewer");
                TempData["SuccessMessage"] = "Review started.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestDocuments(int id, string notes)
        {
            try
            {
                _applicationService.RequestDocuments(id, User.Identity?.Name ?? "Reviewer", notes);
                TempData["SuccessMessage"] = "Documents requested from citizen.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            try
            {
                _applicationService.ApproveApplication(id, User.Identity?.Name ?? "Reviewer");
                TempData["SuccessMessage"] = "Application approved.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id, string rejectionReason)
        {
            try
            {
                _applicationService.RejectApplication(id, User.Identity?.Name ?? "Reviewer", rejectionReason);
                TempData["SuccessMessage"] = "Application rejected.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult PendingReview()
        {
            var applications = _applicationService.GetPendingApplications();
            var viewModels = applications.Select(a => new ApplicationViewModel
            {
                ApplicationId = a.ApplicationId,
                ApplicationNumber = a.ApplicationNumber,
                Status = a.Status,
                SubmissionDate = a.SubmissionDate,
                CalculatedFee = a.CalculatedFee,
                CitizenName = a.Citizen != null ? a.Citizen.FullName : "N/A",
                ServiceTypeName = a.ServiceType != null ? a.ServiceType.ServiceName : "N/A",
                OfficeName = a.Office != null ? a.Office.OfficeName : "N/A"
            }).ToList();
            return View("Index", viewModels);
        }

        private void PopulateDropdowns(ApplicationViewModel viewModel)
        {
            viewModel.Citizens = _context.Citizens
                .OrderBy(c => c.LastName)
                .ToList()
                .Select(c => new SelectListItem
                {
                    Value = c.CitizenId.ToString(),
                    Text = c.FullName
                });

            viewModel.ServiceTypes = _context.ServiceTypes
                .OrderBy(s => s.ServiceName)
                .ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.ServiceTypeId.ToString(),
                    Text = string.Format("{0} ({1:C2})", s.ServiceName, s.BaseFee)
                });

            viewModel.Offices = _context.PublicOffices
                .OrderBy(o => o.OfficeName)
                .ToList()
                .Select(o => new SelectListItem
                {
                    Value = o.OfficeId.ToString(),
                    Text = string.Format("{0} ({1})", o.OfficeName, o.City)
                });
        }

    }
}
