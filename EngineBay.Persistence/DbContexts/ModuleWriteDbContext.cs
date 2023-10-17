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

        public int SaveChanges(ApplicationUser user)
        {
            return this.SaveChanges();
        }

        /// <inheritdoc/>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task<int> SaveChangesAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            return this.SaveChangesAsync(cancellationToken);
        }
    }
}