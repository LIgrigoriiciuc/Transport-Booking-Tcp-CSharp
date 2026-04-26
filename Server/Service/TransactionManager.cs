using Microsoft.Data.Sqlite;
using Server.Util;

namespace Server.Service;


public class TransactionManager
{
    public void Run(Action work)
    {
        using var holder = DatabaseConnection.GetConnection();
        var conn = holder.Connection;
        using var transaction = conn.BeginTransaction();
        DatabaseConnection.BindConnection(conn, transaction);
        try
        {
            work();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new Exception($"Transaction failed: {ex.Message}", ex);
        }
        finally
        {
            DatabaseConnection.UnbindConnection();
        }
    }

}