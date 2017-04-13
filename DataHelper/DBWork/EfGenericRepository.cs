using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using DataHelper.DBWork;



public class EfGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    public EfGenericRepository(DbContext dbContext)
    {
        if (dbContext == null)
            throw new ArgumentNullException("Null DbContext");
        DbContext = dbContext;

        DbSet = DbContext.Set<TEntity>();
    }

    protected DbContext DbContext { get; set; }

    protected DbSet<TEntity> DbSet { get; set; }

    private string ConnectionString
    {
        get
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["MusicContext"].ConnectionString;
        }
    }

    private bool TestConnection()
    {
        return DBHelper.TestConnection(ConnectionString);
    }

    public TEntity Create()
    {
        TestConnection();
        return DbSet.Create();
    }

    public virtual TEntity Add(TEntity entity)
    {
        TestConnection();
        DbEntityEntry dbEntityEntry = DbContext.Entry(entity);

        if (dbEntityEntry.State != EntityState.Detached)
        {
            dbEntityEntry.State = EntityState.Added;
            return entity;
        }

        return DbSet.Add(entity);

    }

    public bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        TestConnection();
        return DbSet.Any(predicate);
    }

    public virtual TEntity GetById(int id)
    {
        TestConnection();
        TEntity t = DbSet.Find(id);
        return t;
    }


    public virtual void Delete(TEntity entity)
    {
        TestConnection();
        DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
        if (dbEntityEntry.State != EntityState.Deleted)
        {
            dbEntityEntry.State = EntityState.Deleted;
        }
        else
        {
            DbSet.Attach(entity);
            DbSet.Remove(entity);
        }
    }

    public virtual void Delete(int id)
    {
        TestConnection();

        TEntity entity = GetById(id);
        if (entity == null) return; // not found; assume already deleted.
        Delete(entity);
    }

    public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        TestConnection();
        TEntity t = DbSet.FirstOrDefault(predicate);
        return t;
    }

    public virtual IQueryable<TEntity> GetAll()
    {
        TestConnection();
        return DbSet.AsQueryable();
    }

    public virtual TEntity Update(TEntity entity)
    {
        TestConnection();
        DbEntityEntry updatedEntityEntry = DbContext.Entry(entity);

        if (DbSet.Local.Any())
        {
            var inContextEntity = DbSet.Find(entity.GetType().GetProperty("Id").GetValue(entity, null));
            DbEntityEntry existentEntityEntry = DbContext.Entry(inContextEntity);
            existentEntityEntry.State = EntityState.Detached;

        }

        if (updatedEntityEntry.State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }

        updatedEntityEntry.State = EntityState.Modified;

        return entity;
    }

    public virtual IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        //var objectContext = ((IObjectContextAdapter)DbContext).ObjectContext;
        //var refreshableObjects = DbContext.ChangeTracker.Entries().Select(c => c.Entity).ToList();
        //objectContext.Refresh(RefreshMode.StoreWins, refreshableObjects);  
        TestConnection();
        return DbSet.Where(predicate).AsQueryable();
    }



}

