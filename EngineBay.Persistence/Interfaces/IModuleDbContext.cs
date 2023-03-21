namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public partial interface IModuleDbContext : IDisposable
    {
        public void MasterOnModelCreating(ModelBuilder builder);
    }
}