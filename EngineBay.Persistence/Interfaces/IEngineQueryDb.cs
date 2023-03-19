namespace EngineBay.Persistence
{
    public interface IEngineQueryDb : IEngineDb
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}