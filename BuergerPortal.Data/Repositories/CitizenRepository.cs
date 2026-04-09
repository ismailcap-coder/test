using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Data.Repositories
{
    public class CitizenRepository : IRepository<Citizen>
    {
        private readonly BuergerPortalContext _context;

        public CitizenRepository(BuergerPortalContext context)
        {
            _context = context;
        }

        public virtual Citizen GetById(int id)
        {
            return _context.Citizens.Find(id);
        }

        public virtual Citizen GetByIdWithApplications(int id)
        {
            return _context.Citizens
                .Include(c => c.Applications)
                .FirstOrDefault(c => c.CitizenId == id);
        }

        public virtual Citizen GetByTaxId(string taxId)
        {
            return _context.Citizens.FirstOrDefault(c => c.TaxId == taxId);
        }

        public virtual IList<Citizen> GetAll()
        {
            return _context.Citizens.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();
        }

        public virtual IList<Citizen> Find(Expression<Func<Citizen, bool>> predicate)
        {
            return _context.Citizens.Where(predicate).ToList();
        }

        public virtual IList<Citizen> SearchByName(string searchTerm)
        {
            return _context.Citizens
                .Where(c => c.LastName.Contains(searchTerm) || c.FirstName.Contains(searchTerm))
                .OrderBy(c => c.LastName)
                .ToList();
        }

        public virtual void Add(Citizen entity)
        {
            _context.Citizens.Add(entity);
            _context.SaveChanges();
        }

        public virtual void Update(Citizen entity)
        {
            _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }

        public virtual void Delete(Citizen entity)
        {
            _context.Citizens.Remove(entity);
            _context.SaveChanges();
        }

        public virtual int Count()
        {
            return _context.Citizens.Count();
        }

        public virtual int Count(Expression<Func<Citizen, bool>> predicate)
        {
            return _context.Citizens.Count(predicate);
        }
    }
}
