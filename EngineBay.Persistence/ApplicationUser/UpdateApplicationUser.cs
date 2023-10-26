namespace EngineBay.Persistence
{
    using EngineBay.Core;

    public class UpdateApplicationUser : ICommandHandler<UpdateApplicationUserCommand, ApplicationUserDto>
    {
        private readonly ModuleWriteDbContext writeDbContext;

        public UpdateApplicationUser(ModuleWriteDbContext writeDbContext)
        {
            this.writeDbContext = writeDbContext;
        }

        public async Task<ApplicationUserDto> Handle(UpdateApplicationUserCommand inputParameters, CancellationToken cancellation)
        {
            if (inputParameters is null)
            {
                throw new ArgumentNullException(nameof(inputParameters));
            }

            var userModel = inputParameters.ToDomainModel();

            await this.writeDbContext.AddAsync(userModel, cancellation);
            await this.writeDbContext.SaveChangesAsync(cancellation);

            return new ApplicationUserDto(userModel);
        }
    }
}
