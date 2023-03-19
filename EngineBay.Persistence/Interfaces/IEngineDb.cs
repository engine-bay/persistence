namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public partial interface IEngineDb : IDisposable
    {
        public void MasterOnModelCreating(ModelBuilder modelBuilder);
    }
}