namespace EngineBay.Persistence
{
    public interface IDatabaseModule
    {
        public IReadOnlyCollection<IModuleDbContext> GetRegisteredDbContexts(IDbContextOptionsFactory dbContextOptionsFactory);
    }
}