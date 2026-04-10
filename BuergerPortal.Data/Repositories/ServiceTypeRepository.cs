using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Data.Repositories
{
    public class ServiceTypeRepository : IRepository<ServiceType>
    {
        private readonly BuergerPortalContext _context;

        public ServiceTypeRepository(BuergerPortalContext context)
        {
            _context = context;
        }

        public virtual ServiceType? GetById(int id)
        {
            return _context.ServiceTypes.Find(id);
        }

        public virtual ServiceType? GetByCode(string serviceCode)
        {
            return _context.ServiceTypes.FirstOrDefault(s => s.ServiceCode == serviceCode);
        }

        public virtual IList<ServiceType> GetAll()
        {
            return _context.ServiceTypes.OrderBy(s => s.ServiceName).ToList();
        }

        public virtual IList<ServiceType> GetByCategory(int category)
        {
            return _context.ServiceTypes.Where(s => (int)s.Category == category).ToList();
        }

        public virtual IList<ServiceType> Find(Expression<Func<ServiceType, bool>> predicate)
        {
            return _context.ServiceTypes.Where(predicate).ToList();
        }

        public virtual void Add(ServiceType entity)
        {
            _context.ServiceTypes.Add(entity);
            _context.SaveChanges();
        }

        public virtual void Update(ServiceType entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public virtual void Delete(ServiceType entity)
        {
            _context.ServiceTypes.Remove(entity);
            _context.SaveChanges();
        }

        public virtual int Count()
        {
            return _context.ServiceTypes.Count();
        }

        public virtual int Count(Expression<Func<ServiceType, bool>> predicate)
        {
            return _context.ServiceTypes.Count(predicate);
        }
    }
}
