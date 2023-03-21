namespace EngineBay.Persistence
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public string? CustomTag { get; set; }
    }
}