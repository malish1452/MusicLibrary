using System;

public interface IGenericRepository<T> where T : class
{
    T Create();

    IQueryable<T> GetAll();

    //           IQueryable<T> GetAllNoTracking();

    IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    //            IQueryable<T> ReloadWhere(Expression<Func<T, bool>> predicate);

    //           IQueryable<T> SelectNWhere(Expression<Func<T, bool>> conditionPredicate, Expression<Func<T, int>> orderPredicate, int count);

    //           IQueryable<T> SelectNWhereNoTracking(Expression<Func<T, bool>> conditionPredicate, Expression<Func<T, int>> orderPredicate, int count);

    //            IQueryable<T> WhereNoTracking(Expression<Func<T, bool>> predicate);

    //            IQueryable<T> WhereExpandable(Expression<Func<T, bool>> predicate);

    //            IQueryable<T> ReloadWhereExpandable(Expression<Func<T, bool>> predicate);

    T FirstOrDefault(Expression<Func<T, bool>> predicate);

    T GetById(int id);

    //            T GetReloadedById(int id);

    //            T ReloadEntity(T entity);

    T Add(T entity);

    //            void AddRange(IEnumerable<T> entities);

    //            void AddBulkRange(IEnumerable<T> entities, string tableName, List<KeyValuePair<string, string>> mappings, string additionalSql);

    T Update(T entity);

    void Delete(T entity);

    void Delete(int id);

    //            void DeleteAll(IEnumerable<T> entities);

    //            DateTime? WhereMaxDate(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, DateTime?>> maxPredicate);

    //            DateTime? WhereMinDate(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, DateTime?>> minPredicate);

    bool Any(Expression<Func<T, bool>> predicate);

    //            IQueryable<T> Include(params Expression<Func<T, object>>[] includes);
}

