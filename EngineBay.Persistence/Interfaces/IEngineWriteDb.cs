namespace EngineBay.Persistence
{
    public interface IEngineWriteDb : IEngineQueryDb
    {
        new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}