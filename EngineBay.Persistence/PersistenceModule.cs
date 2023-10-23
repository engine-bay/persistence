namespace EngineBay.Persistence
{
    using EngineBay.Core;

    public class PersistenceModule : BaseModule
    {
        public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TimestampInterceptor>();

            return services;
        }

        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            return endpoints;
        }

        public WebApplication AddMiddleware(WebApplication app)
        {
            return app;
        }
    }
}