using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Repository.EF;
using Server.Util;

namespace Server.Repository.EF;
public class EFSeatRepository: EFGenericRepository<long, Seat>
{
    public override List<Seat> Filter(Filter filter)
    {
        using var ctx = CreateContext();
        IQueryable<Seat> query = ctx.Set<Seat>();
        foreach (var (column, value) in filter.GetConditions())
        {
            switch (column)
            {
                case "trip_id":
                    var tripId = Convert.ToInt64(value);
                    query = query.Where(s => s.TripId == tripId);
                    break;
                case "reservation_id":
                    var resId = Convert.ToInt64(value);
                    query = query.Where(s => s.ReservationId == resId);
                    break;
                case "isReserved":
                    var reserved = Convert.ToInt32(value) == 1;
                    query = query.Where(s => s.IsReserved == reserved);
                    break;
            }
        }
        return query.ToList();
    }
}