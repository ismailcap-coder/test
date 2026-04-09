using BuergerPortal.Business.Services;
using BuergerPortal.Business.Validators;
using BuergerPortal.Data;
using BuergerPortal.Data.Repositories;
using BuergerPortal.Domain.Entities;
using BuergerPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BuergerPortal.Web.Controllers
{
    public class CitizenController : Controller
    {
        private readonly ICitizenService _citizenService;
        private readonly BuergerPortalContext _context;

        public CitizenController()
        {
            _context = new BuergerPortalContext();
            var repo = new CitizenRepository(_context);
            var validator = new CitizenValidator();
            _citizenService = new CitizenService(repo, validator);
        }

        public ActionResult Index(string searchTerm)
        {
            var citizens = string.IsNullOrEmpty(searchTerm)
                ? _citizenService.GetAllCitizens()
                : _citizenService.SearchCitizens(searchTerm);

            var viewModels = new List<CitizenViewModel>();

            foreach (var c in citizens)
            {
                var vm = new CitizenViewModel
                {
                    CitizenId = c.CitizenId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    DateOfBirth = c.DateOfBirth,
                    City = c.City,
                    TaxId = c.TaxId,
                    IsLowIncome = c.IsLowIncome,
                    RegistrationDate = c.RegistrationDate
                };

                viewModels.Add(vm);
            }

            ViewData["SearchTerm"] = searchTerm;
            return View(viewModels);
        }

        public ActionResult Details(int id)
        {
            var citizen = _citizenService.GetCitizenWithApplications(id);
            var viewModel = MapToViewModel(citizen);
            viewModel.ApplicationCount = citizen.Applications != null ? citizen.Applications.Count : 0;
            return View(viewModel);
        }

        public ActionResult Create()
        {
            return View(new CitizenViewModel { DateOfBirth = new DateTime(1980, 1, 1) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CitizenViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var citizen = MapToEntity(viewModel);
                    _citizenService.CreateCitizen(citizen);
                    TempData["SuccessMessage"] = "Citizen registered successfully.";
                    return RedirectToAction("Index");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(viewModel);
        }

        public ActionResult Edit(int id)
        {
            var citizen = _citizenService.GetCitizen(id);
            return View(MapToViewModel(citizen));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CitizenViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var citizen = MapToEntity(viewModel);
                    _citizenService.UpdateCitizen(citizen);
                    TempData["SuccessMessage"] = "Citizen updated successfully.";
                    return RedirectToAction("Index");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(viewModel);
        }

        public ActionResult Delete(int id)
        {
            var citizen = _citizenService.GetCitizen(id);
            return View(MapToViewModel(citizen));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _citizenService.DeleteCitizen(id);
                TempData["SuccessMessage"] = "Citizen deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Delete", new { id = id });
            }
        }

        private CitizenViewModel MapToViewModel(Citizen citizen)
        {
            return new CitizenViewModel
            {
                CitizenId = citizen.CitizenId,
                FirstName = citizen.FirstName,
                LastName = citizen.LastName,
                DateOfBirth = citizen.DateOfBirth,
                StreetAddress = citizen.StreetAddress,
                City = citizen.City,
                PostalCode = citizen.PostalCode,
                PhoneNumber = citizen.PhoneNumber,
                Email = citizen.Email,
                TaxId = citizen.TaxId,
                IsLowIncome = citizen.IsLowIncome,
                RegistrationDate = citizen.RegistrationDate
            };
        }

        private Citizen MapToEntity(CitizenViewModel viewModel)
        {
            return new Citizen
            {
                CitizenId = viewModel.CitizenId,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                DateOfBirth = viewModel.DateOfBirth,
                StreetAddress = viewModel.StreetAddress,
                City = viewModel.City,
                PostalCode = viewModel.PostalCode,
                PhoneNumber = viewModel.PhoneNumber,
                Email = viewModel.Email,
                TaxId = viewModel.TaxId,
                IsLowIncome = viewModel.IsLowIncome,
                RegistrationDate = viewModel.RegistrationDate
            };
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
