namespace EngineBay.Persistence
{
    public interface IModuleQueryDbContext : IModuleDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}