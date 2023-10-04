namespace EngineBay.Persistence
{
    public class ApplicationUserDto
    {
        public ApplicationUserDto(ApplicationUser applicationUser)
        {
            ArgumentNullException.ThrowIfNull(applicationUser, nameof(applicationUser));

            this.Id = applicationUser.Id;
            this.Username = applicationUser.Username;
        }

        public Guid Id { get; set; }

        public string Username { get; set; }
    }
}
