namespace EngineBay.Persistence.Tests
{
    public class MockApplicationUser : ApplicationUser
    {
        public MockApplicationUser()
        {
            this.Id = Guid.NewGuid();
            this.Username = "MockUser";
            this.CreatedById = default(Guid);
            this.LastUpdatedById = default(Guid);
        }
    }
}
