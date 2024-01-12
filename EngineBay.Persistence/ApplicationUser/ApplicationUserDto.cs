namespace EngineBay.Persistence
{
    public class ApplicationUserDto
    {
        public ApplicationUserDto()
        {
            this.Id = Guid.Empty;
            this.Username = string.Empty;
        }

        public ApplicationUserDto(ApplicationUser applicationUser)
        {
            ArgumentNullException.ThrowIfNull(applicationUser);

            this.Id = applicationUser.Id;
            this.Username = applicationUser.Username;
        }

        public Guid Id { get; set; }

        public string Username { get; set; }
    }
}
