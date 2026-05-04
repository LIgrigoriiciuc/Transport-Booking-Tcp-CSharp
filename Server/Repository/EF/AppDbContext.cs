using Microsoft.Data.Sqlite;
using Server.Domain;
using Shared.Util;

namespace Server.Repository.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
public class AppDbContext : DbContext
{
    //for sharing an existing connection inside a transaction
    private readonly SqliteConnection? _connection;
 
    //outside a transaction where EF manages its own connection
    public AppDbContext() { }
 
    //reuses the transaction's connection
    public AppDbContext(SqliteConnection connection)
    {
        _connection = connection;
    }
    //new each time
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<Seat> Seats { get; set; } = null!;
 
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_connection != null)
            optionsBuilder.UseSqlite(_connection);
        else
            optionsBuilder.UseSqlite("Data Source=transport.db");
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
    }
    //runs once
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("reservations");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(r => r.ClientName).HasColumnName("clientName").IsRequired();
            entity.Property(r => r.UserId).HasColumnName("userId");
            entity.Property(r => r.ReservationTime).HasColumnName("reservationTime").HasConversion(v => DateTimeUtils.FormatDateTime(v),
                      v => DateTimeUtils.Parse(v));
        });
 
        modelBuilder.Entity<Seat>(entity =>
        {
            entity.ToTable("seats");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(s => s.Number).HasColumnName("number");
            entity.Property(s => s.IsReserved).HasColumnName("isReserved").HasConversion<int>();
            entity.Property(s => s.TripId).HasColumnName("trip_id");
            entity.Property(s => s.ReservationId).HasColumnName("reservation_id").IsRequired(false);
        });
    }
}