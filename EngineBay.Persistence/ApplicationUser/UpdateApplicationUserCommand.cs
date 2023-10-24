namespace EngineBay.Persistence
{
    public class UpdateApplicationUserCommand
    {
        required public string Username { get; set; }

        public ApplicationUser ToDomainModel()
        {
            return new ApplicationUser(this.Username);
        }
    }
}
