using Server.Domain;

namespace Server.Service;


public class FacadeService
{
    private readonly AuthService _userService;
    private readonly TripService _tripService;
    private readonly SeatService _seatService;
    private readonly ReservationService _reservationService;
    private readonly OfficeService _officeService;
    private readonly TransactionManager _txManager;

    public FacadeService(
        AuthService userService,
        TripService tripService,
        SeatService seatService,
        ReservationService reservationService,
        OfficeService officeService,
        TransactionManager txManager)
    {
        _userService = userService;
        _tripService = tripService;
        _seatService = seatService;
        _reservationService = reservationService;
        _officeService = officeService;
        _txManager = txManager;
    }

    public User Login(string user, string pass)
        => _userService.Login(user, pass);
    
    // public void Logout() => _userService.Logout();

    public List<Trip> SearchTrips(string destination, DateTime? from, DateTime? to) 
        => _tripService.Search(destination, from, to);

    public List<Seat> GetSeatsForTrip(long tripId) 
        => _seatService.GetByTripId(tripId);

    public void MakeReservationForSeats(string clientName, List<Seat> chosenSeats)
    {
        _txManager.Run(() => _reservationService.ReserveSeats(clientName, chosenSeats));
    }

    public void CancelReservation(long reservationId)
    {
        _txManager.Run(() => _reservationService.Cancel(reservationId));
    }

    public List<Reservation> GetAllReservations() 
        => _reservationService.GetAll();
    public Office? GetOfficeById(long id)
        => _officeService.FindById(id);
 
    public int CountFreeSeats(long tripId)
        => _seatService.GetFreeByTripId(tripId).Count;
    public Seat? GetSeatById(long seatId)
        => _seatService.FindById(seatId);
 
    public Reservation? GetReservationById(long reservationId)
        => _reservationService.FindById(reservationId);
    public User? GetUserById(long userId)
        => _userService.FindById(userId);
 
    public List<int> GetSeatNumbersByReservation(long reservationId)
        => _seatService.GetSeatNumbersByReservation(reservationId);
 
    public long? GetTripIdByReservation(long reservationId)
        => _seatService.GetTripIdByReservationId(reservationId);
}