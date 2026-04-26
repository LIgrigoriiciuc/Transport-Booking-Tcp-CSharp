namespace Server.Domain;

public class Reservation : Entity<long>
{
    public string ClientName { get; set; }
    public DateTime ReservationTime { get; set; }
    public long UserId { get; set; }
 
    public Reservation(string clientName, long userId) : base()
    {
        ClientName = clientName;
        ReservationTime = DateTime.Now;
        UserId = userId;
    }
 
    public Reservation(long id, string clientName, long userId, DateTime reservationTime) : base(id)
    {
        ClientName = clientName;
        UserId = userId;
        ReservationTime = reservationTime;
    }
}
