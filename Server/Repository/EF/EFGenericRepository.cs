using Server.Domain;
using Microsoft.EntityFrameworkCore;
using Server.Util;

namespace Server.Repository.EF;
public abstract class EFGenericRepository<TId, TE> : IRepository<TId, TE> where TE : Entity<TId>
{
    protected AppDbContext CreateContext()
    {
        var tx = DatabaseConnection.GetActiveTransaction();
        if (tx != null)
        {
            var ctx = new AppDbContext(tx.Connection!);
            ctx.Database.UseTransaction(tx);
            return ctx;
        }
        return new AppDbContext();
    }

    public void Add(TE entity)
    {
        using var ctx = CreateContext();
        ctx.Set<TE>().Add(entity);
        ctx.SaveChanges(); 
    }

    public bool Remove(TId id)
    {
        using var ctx = CreateContext();
        var entity = ctx.Set<TE>().Find(id);
        if (entity is null) return false;
        ctx.Set<TE>().Remove(entity);
        return ctx.SaveChanges() > 0;
    }

    public bool Update(TE entity)
    {
        using var ctx = CreateContext();
        ctx.Set<TE>().Update(entity);
        return ctx.SaveChanges() > 0;
    }

    public TE? FindById(TId id)
    {
        using var ctx = CreateContext();
        return ctx.Set<TE>().Find(id);
    }

    public List<TE> GetAll()
    {
        using var ctx = CreateContext();
        return ctx.Set<TE>().ToList();
    }
    public abstract List<TE> Filter(Filter filter);
}