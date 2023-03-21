namespace EngineBay.Persistence
{
    public interface IModuleWriteDbContext : IModuleQueryDbContext
    {
        int SaveChanges(ApplicationUser user);

        new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    }
}