using System.Transactions;
using Microsoft.Data.Sqlite;
using Server.Domain;
using Server.Util;

namespace Server.Repository;

public abstract class GenericRepository<TId, TE> where TE : Entity<TId>
{
    private SqliteCommand CreateCommand(string sql, SqliteConnection conn)
    {
        var cmd = new SqliteCommand(sql, conn);
        cmd.Transaction = DatabaseConnection.GetActiveTransaction();
        return cmd;
    }
 
    public List<TE> Filter(Filter filter)
    {
        var entities = new List<TE>();
        string sql = $"SELECT * FROM {GetTableName()} {filter.BuildWhere()}";
 
        using var holder = DatabaseConnection.GetConnection();
        using var command = CreateCommand(sql, holder.Connection);
        filter.ApplyParameters(command);
 
        using var reader = command.ExecuteReader();
        while (reader.Read())
            entities.Add(MapResultSetToEntity(reader));
 
        return entities;
    }
 
    public List<TE> GetAll()
    {
        var entities = new List<TE>();
        string sql = $"SELECT * FROM {GetTableName()}";
 
        using var holder = DatabaseConnection.GetConnection();
        using var command = CreateCommand(sql, holder.Connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
            entities.Add(MapResultSetToEntity(reader));
 
        return entities;
    }
 
    public TE? FindById(TId id)
    {
        string sql = $"SELECT * FROM {GetTableName()} WHERE id = @id";
 
        using var holder = DatabaseConnection.GetConnection();
        using var command = CreateCommand(sql, holder.Connection);
        command.Parameters.AddWithValue("@id", id);
 
        using var reader = command.ExecuteReader();
        return reader.Read() ? MapResultSetToEntity(reader) : null;
    }
 
    public bool Remove(TId id)
    {
        string sql = $"DELETE FROM {GetTableName()} WHERE id = @id";
 
        using var holder = DatabaseConnection.GetConnection();
        using var command = CreateCommand(sql, holder.Connection);
        command.Parameters.AddWithValue("@id", id);
        return command.ExecuteNonQuery() > 0;
    }
 
    public void Add(TE e)
    {
        string sql = $"{BuildInsertSql()}; SELECT last_insert_rowid();";
 
        using var holder = DatabaseConnection.GetConnection();
        using var command = CreateCommand(sql, holder.Connection);
        SetInsertParameters(command, e);
 
        var result = command.ExecuteScalar();
        if (result != null)
            e.Id = (TId)Convert.ChangeType(result, typeof(TId));
    }
 
    public bool Update(TE e)
    {
        using var holder = DatabaseConnection.GetConnection();
        using var command = CreateCommand(BuildUpdateSql(), holder.Connection);
        SetUpdateParameters(command, e);
        return command.ExecuteNonQuery() > 0;
    }
 
    protected abstract string BuildInsertSql();
    protected abstract void SetInsertParameters(SqliteCommand command, TE e);
    protected abstract string BuildUpdateSql();
    protected abstract void SetUpdateParameters(SqliteCommand command, TE e);
    public abstract string GetTableName();
    protected abstract TE MapResultSetToEntity(SqliteDataReader reader);
}