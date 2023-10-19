namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public class ModuleWriteDbContext : ModuleDbContext, IModuleWriteDbContext
    {
        private readonly TimestampInterceptor timestampInterceptor;

        public ModuleWriteDbContext(DbContextOptions<ModuleWriteDbContext> options, TimestampInterceptor timestampInterceptor)
            : base(options)
        {
            this.timestampInterceptor = timestampInterceptor;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ArgumentNullException.ThrowIfNull(optionsBuilder);

            optionsBuilder.AddInterceptors(this.timestampInterceptor);

            base.OnConfiguring(optionsBuilder);
        }
    }
}