using System;
using System.Linq;
using System.Linq.Expressions;

namespace DataHelper.DBWork
{



    public interface IGenericRepository<T> where T : class
    {
        T Create();

        IQueryable<T> GetAll();

        IQueryable<T> Where(Expression<Func<T, bool>> predicate);

        T FirstOrDefault(Expression<Func<T, bool>> predicate);

        T GetById(int id);

        T Add(T entity);

        T Update(T entity);

        void Delete(T entity);

        void Delete(int id);

        bool Any(Expression<Func<T, bool>> predicate);

    }
}

