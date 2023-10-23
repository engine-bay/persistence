namespace EngineBay.Persistence.Tests.Mocks
{
    using System;
    using EngineBay.Core;

    public class MockCurrentIdentity : ICurrentIdentity
    {
        public MockCurrentIdentity(ApplicationUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            this.UserId = user.Id;
            this.Username = user.Username;
        }

        public string Username { get; }

        public Guid UserId { get; }
    }
}
