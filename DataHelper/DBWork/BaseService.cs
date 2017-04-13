namespace DataHelper.DBWork
{
    public class BaseService
    {
        protected IDataProvider Db;

        public BaseService(IDataProvider db)
        {
            Db = db;
        }

    }
}