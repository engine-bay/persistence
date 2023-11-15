namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public interface IDatabaseModule
    {
        public IReadOnlyCollection<IModuleDbContext> GetRegisteredDbContexts(DbContextOptions<ModuleWriteDbContext> dbOptions);
    }
}