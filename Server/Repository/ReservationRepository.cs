using System.Transactions;
using Microsoft.Data.Sqlite;
using Server.Domain;
using Shared.Util;

namespace Server.Repository;

public class ReservationRepository : GenericRepository<long, Reservation>
{
    public override string GetTableName() => "reservations";
 
    protected override string BuildInsertSql() =>
        "INSERT INTO reservations (clientName, userId, reservationTime) VALUES (@clientName, @userId, @reservationTime)";
 
    protected override void SetInsertParameters(SqliteCommand command, Reservation reservation)
    {
        command.Parameters.AddWithValue("@clientName", reservation.ClientName);
        command.Parameters.AddWithValue("@userId", reservation.UserId);
        command.Parameters.AddWithValue("@reservationTime", DateTimeUtils.FormatDateTime(reservation.ReservationTime));
    }
 
    protected override string BuildUpdateSql() =>
        "UPDATE reservations SET clientName = @clientName, userId = @userId, reservationTime = @reservationTime WHERE id = @id";
 
    protected override void SetUpdateParameters(SqliteCommand command, Reservation reservation)
    {
        command.Parameters.AddWithValue("@clientName", reservation.ClientName);
        command.Parameters.AddWithValue("@userId", reservation.UserId);
        command.Parameters.AddWithValue("@reservationTime", DateTimeUtils.FormatDateTime(reservation.ReservationTime));
        command.Parameters.AddWithValue("@id", reservation.Id);
    }
 
    protected override Reservation MapResultSetToEntity(SqliteDataReader reader)
    {
        long id = reader.GetInt64(reader.GetOrdinal("id"));
        string clientName = reader.GetString(reader.GetOrdinal("clientName"));
        long userId = reader.GetInt64(reader.GetOrdinal("userId"));
        DateTime reservationTime = DateTimeUtils.Parse(reader.GetString(reader.GetOrdinal("reservationTime")));
        return new Reservation(id, clientName, userId, reservationTime);
    }
}
