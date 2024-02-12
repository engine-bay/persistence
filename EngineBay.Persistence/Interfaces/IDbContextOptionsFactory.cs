namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public interface IDbContextOptionsFactory
    {
        DbContextOptions<TDbContext> GetDbContextOptions<TDbContext>()
            where TDbContext : DbContext;
    }
}