namespace EngineBay.Persistence
{
    using System;

    public class ApplicationUserDto
    {
        public ApplicationUserDto(ApplicationUser applicationUser)
        {
            if (applicationUser is null)
            {
                throw new ArgumentNullException(nameof(applicationUser));
            }

            this.Id = applicationUser.Id;
            this.Name = applicationUser.Name;
        }

        public Guid Id { get; set; }

        public string? Name { get; set; }
    }
}