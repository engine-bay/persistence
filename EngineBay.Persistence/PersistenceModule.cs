namespace EngineBay.Persistence
{
    using EngineBay.Core;

    public class PersistenceModule : BaseModule
    {
        public override IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<AuditableModelInterceptor>();

            return services;
        }
    }
}