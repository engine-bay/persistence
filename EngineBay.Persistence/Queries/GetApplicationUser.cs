namespace EngineBay.Persistence
{
    using System.Security.Claims;
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class GetApplicationUser : IQueryHandler<ClaimsPrincipal, ApplicationUser>
    {
        private readonly ModuleDbContext db;

        public GetApplicationUser(ModuleDbContext db)
        {
            this.db = db;
        }

        /// <inheritdoc/>
        public async Task<ApplicationUser> Handle(ClaimsPrincipal user, CancellationToken cancellation)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.Identity is null)
            {
                throw new ArgumentException(nameof(user.Identity));
            }

            var applicationUser = await this.db.ApplicationUsers.FirstOrDefaultAsync(x => x.Name == user.Identity.Name, cancellation).ConfigureAwait(false);

            if (applicationUser is null)
            {
                throw new ArgumentException(nameof(applicationUser));
            }

            return applicationUser;
        }
    }
}