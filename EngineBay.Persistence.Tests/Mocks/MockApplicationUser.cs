namespace EngineBay.Persistence.Tests
{
    public class MockApplicationUser : ApplicationUser
    {
        public MockApplicationUser()
            : base("MockUser")
        {
            this.Id = Guid.NewGuid();
            this.CreatedById = default(Guid);
            this.LastUpdatedById = default(Guid);
        }
    }
}
