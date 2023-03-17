namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public class EngineQueryDb : EngineDb, IEngineQueryDb
    {
        public EngineQueryDb(DbContextOptions<EngineQueryDb> options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new InvalidOperationException($"Tried to save changes on a read only db context {nameof(EngineQueryDb)}");
        }
    }
}