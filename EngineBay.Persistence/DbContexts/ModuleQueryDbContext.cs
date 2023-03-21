namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public class ModuleQueryDbContext : ModuleDbContext, IModuleQueryDbContext
    {
        public ModuleQueryDbContext(DbContextOptions<ModuleQueryDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        public override int SaveChanges()
        {
            throw new InvalidOperationException($"Tried to save changes on a read only db context {nameof(ModuleQueryDbContext)}");
        }

        /// <inheritdoc/>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new InvalidOperationException($"Tried to save changes on a read only db context {nameof(ModuleQueryDbContext)}");
        }
    }
}