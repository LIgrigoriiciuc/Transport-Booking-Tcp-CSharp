using Server.Domain;
using Server.Repository;

namespace Server.Service;

public class OfficeService : AbstractService<long, Office>
{
    public OfficeService(IRepository<long,Office> repository) : base(repository)
    {
    }
}