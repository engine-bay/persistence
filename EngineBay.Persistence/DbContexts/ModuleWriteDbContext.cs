namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public class ModuleWriteDbContext : ModuleDbContext, IModuleWriteDbContext
    {
        public ModuleWriteDbContext(DbContextOptions<ModuleWriteDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        /// <inheritdoc/>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}