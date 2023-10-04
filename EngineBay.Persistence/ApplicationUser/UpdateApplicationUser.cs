namespace EngineBay.Persistence
{
    using System.Security.Claims;
    using EngineBay.Core;

    public class UpdateApplicationUser : ICommandHandler<UpdateApplicationUserCommand, ApplicationUserDto>
    {
        private readonly ModuleWriteDbContext writeDbContext;

        public UpdateApplicationUser(ModuleWriteDbContext writeDbContext)
        {
            this.writeDbContext = writeDbContext;
        }

        public async Task<ApplicationUserDto> Handle(UpdateApplicationUserCommand inputParameters, ClaimsPrincipal user, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(inputParameters, nameof(inputParameters));

            var userModel = inputParameters.ToDomainModel();

            await this.writeDbContext.AddAsync(userModel, cancellation).ConfigureAwait(false);
            await this.writeDbContext.SaveChangesAsync(cancellation).ConfigureAwait(false);

            return new ApplicationUserDto(userModel);
        }
    }
}
