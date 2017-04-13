using System;

public class DBHelper
{
    public static bool TestConnection(string connectionString)
    {
        using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Suppress))
        {
            using (var sqlConn = new SqlConnection(connectionString + ";Connection Timeout=2"))
            {
                try
                {
                    sqlConn.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception("Связь с базой данных отсутствует...", ex);
                }
            }
        }
        return true;
    }
}

