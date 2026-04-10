using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using BuergerPortal.Domain.Entities;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Data.Repositories
{
    public class ServiceApplicationRepository : IRepository<ServiceApplication>
    {
        private readonly BuergerPortalContext _context;

        public ServiceApplicationRepository(BuergerPortalContext context)
        {
            _context = context;
        }

        public virtual ServiceApplication? GetById(int id)
        {
            return _context.ServiceApplications.Find(id);
        }

        public virtual ServiceApplication? GetByIdWithDetails(int id)
        {
            return _context.ServiceApplications
                .Include(a => a.Citizen)
                .Include(a => a.ServiceType)
                .Include(a => a.Office)
                .Include(a => a.Documents)
                .Include(a => a.AuditLogs)
                .FirstOrDefault(a => a.ApplicationId == id);
        }

        public virtual ServiceApplication? GetByApplicationNumber(string applicationNumber)
        {
            return _context.ServiceApplications
                .Include(a => a.Citizen)
                .Include(a => a.ServiceType)
                .FirstOrDefault(a => a.ApplicationNumber == applicationNumber);
        }

        public virtual IList<ServiceApplication> GetAll()
        {
            return _context.ServiceApplications
                .Include(a => a.Citizen)
                .Include(a => a.ServiceType)
                .Include(a => a.Office)
                .OrderByDescending(a => a.SubmissionDate)
                .ToList();
        }

        public virtual IList<ServiceApplication> GetByCitizenId(int citizenId)
        {
            return _context.ServiceApplications
                .Include(a => a.ServiceType)
                .Include(a => a.Office)
                .Where(a => a.CitizenId == citizenId)
                .OrderByDescending(a => a.SubmissionDate)
                .ToList();
        }

        public virtual IList<ServiceApplication> GetByStatus(ApplicationStatus status)
        {
            return _context.ServiceApplications
                .Include(a => a.Citizen)
                .Include(a => a.ServiceType)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.SubmissionDate)
                .ToList();
        }

        public virtual IList<ServiceApplication> GetPendingReview()
        {
            return _context.ServiceApplications
                .Include(a => a.Citizen)
                .Include(a => a.ServiceType)
                .Include(a => a.Office)
                .Where(a => a.Status == ApplicationStatus.Submitted || a.Status == ApplicationStatus.UnderReview)
                .OrderBy(a => a.SubmissionDate)
                .ToList();
        }

        public virtual int GetActiveApplicationCount(int citizenId)
        {
            return _context.ServiceApplications
                .Count(a => a.CitizenId == citizenId
                    && a.Status != ApplicationStatus.Approved
                    && a.Status != ApplicationStatus.Rejected);
        }

        public virtual IList<ServiceApplication> Find(Expression<Func<ServiceApplication, bool>> predicate)
        {
            return _context.ServiceApplications.Where(predicate).ToList();
        }

        public virtual void Add(ServiceApplication entity)
        {
            _context.ServiceApplications.Add(entity);
            _context.SaveChanges();
        }

        public virtual void Update(ServiceApplication entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public virtual void Delete(ServiceApplication entity)
        {
            _context.ServiceApplications.Remove(entity);
            _context.SaveChanges();
        }

        public virtual int Count()
        {
            return _context.ServiceApplications.Count();
        }

        public virtual int Count(Expression<Func<ServiceApplication, bool>> predicate)
        {
            return _context.ServiceApplications.Count(predicate);
        }

        public virtual void AddAuditLog(AuditLog auditLog)
        {
            _context.AuditLogs.Add(auditLog);
            _context.SaveChanges();
        }

        // EF Core LINQ equivalent of the former raw SQL query
        public virtual IList<ServiceApplication> GetApplicationsByRawQuery(string districtCode)
        {
            return _context.ServiceApplications
                .Include(a => a.Office)
                .Where(a => a.Office != null && a.Office.DistrictCode == districtCode)
                .ToList();
        }
    }
}
