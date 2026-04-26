using Server.Domain;
using Server.Repository;

namespace Server.Service;


public class ReservationService : AbstractService<long, Reservation>
{
    private readonly SeatService _seatService;

    public ReservationService(ReservationRepository repository, SeatService seatService) 
        : base(repository)
    {
        _seatService = seatService;
    }

    public void ReserveSeats(string clientName, List<Seat> chosenSeats, long userId)
    {
        if (string.IsNullOrWhiteSpace(clientName))
            throw new ArgumentException("Client name cannot be empty.");
 
        if (chosenSeats == null || chosenSeats.Count == 0)
            throw new ArgumentException("Must select at least one seat.");
 
        var reservation = new Reservation(clientName, userId);
        Repository.Add(reservation);
 
        foreach (var seat in chosenSeats)
        {
            seat.IsReserved = true;
            seat.ReservationId = reservation.Id;
            _seatService.Update(seat);
        }
    }

    public void Cancel(long reservationId)
    {
        var seats = _seatService.GetByReservationId(reservationId);
 
        foreach (var seat in seats)
        {
            seat.IsReserved = false;
            seat.ReservationId = null;
            _seatService.Update(seat);
        }
 
        Repository.Remove(reservationId);
    }
}