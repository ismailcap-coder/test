using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BuergerPortal.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        T? GetById(int id);
        IList<T> GetAll();
        IList<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        int Count();
        int Count(Expression<Func<T, bool>> predicate);
    }
}
