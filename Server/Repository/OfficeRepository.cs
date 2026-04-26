using System.Transactions;
using Microsoft.Data.Sqlite;
using Server.Domain;

namespace Server.Repository;


public class OfficeRepository : GenericRepository<long, Office>
{
    public override string GetTableName() => "offices";
 
    protected override string BuildInsertSql() =>
        "INSERT INTO offices (address) VALUES (@address)";
 
    protected override void SetInsertParameters(SqliteCommand command, Office office)
    {
        command.Parameters.AddWithValue("@address", office.Address);
    }
 
    protected override string BuildUpdateSql() =>
        "UPDATE offices SET address = @address WHERE id = @id";
 
    protected override void SetUpdateParameters(SqliteCommand command, Office office)
    {
        command.Parameters.AddWithValue("@address", office.Address);
        command.Parameters.AddWithValue("@id", office.Id);
    }
 
    protected override Office MapResultSetToEntity(SqliteDataReader reader)
    {
        long id = reader.GetInt64(reader.GetOrdinal("id"));
        string address = reader.GetString(reader.GetOrdinal("address"));
        return new Office(id, address);
    }
}