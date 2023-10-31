namespace EngineBay.Persistence
{
    public interface IModuleWriteDbContext : IModuleQueryDbContext
    {
        int SaveChanges();

        new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}