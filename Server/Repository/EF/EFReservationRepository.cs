using Server.Domain;

namespace Server.Repository.EF;

public class EFReservationRepository : EFGenericRepository<long, Reservation>
{

    public override List<Reservation> Filter(Filter filter)
    {
        using var ctx = CreateContext();
        IQueryable<Reservation> query = ctx.Set<Reservation>();
        foreach (var (column, value) in filter.GetConditions())
        {switch (column)
            {
                case "userId":
                    var uid = Convert.ToInt64(value);
                    query = query.Where(r => r.UserId == uid); //adds condition to query
                    break;
                case "clientName":
                    var name = value.ToString()!;
                    query = query.Where(r => r.ClientName == name);
                    break;
            }
        }
        return query.ToList();
    }
}


    