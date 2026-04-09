using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Data.Repositories
{
    public class PublicOfficeRepository : IRepository<PublicOffice>
    {
        private readonly BuergerPortalContext _context;

        public PublicOfficeRepository(BuergerPortalContext context)
        {
            _context = context;
        }

        public virtual PublicOffice GetById(int id)
        {
            return _context.PublicOffices.Find(id);
        }

        public virtual PublicOffice GetByDistrictCode(string districtCode)
        {
            return _context.PublicOffices.FirstOrDefault(o => o.DistrictCode == districtCode);
        }

        public virtual IList<PublicOffice> GetAll()
        {
            return _context.PublicOffices.OrderBy(o => o.City).ThenBy(o => o.OfficeName).ToList();
        }

        public virtual IList<PublicOffice> Find(Expression<Func<PublicOffice, bool>> predicate)
        {
            return _context.PublicOffices.Where(predicate).ToList();
        }

        public virtual void Add(PublicOffice entity)
        {
            _context.PublicOffices.Add(entity);
            _context.SaveChanges();
        }

        public virtual void Update(PublicOffice entity)
        {
            _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }

        public virtual void Delete(PublicOffice entity)
        {
            _context.PublicOffices.Remove(entity);
            _context.SaveChanges();
        }

        public virtual int Count()
        {
            return _context.PublicOffices.Count();
        }

        public virtual int Count(Expression<Func<PublicOffice, bool>> predicate)
        {
            return _context.PublicOffices.Count(predicate);
        }
    }
}
