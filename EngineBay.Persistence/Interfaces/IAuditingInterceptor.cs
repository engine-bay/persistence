namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public interface IAuditingInterceptor : ISaveChangesInterceptor
    {
    }
}
