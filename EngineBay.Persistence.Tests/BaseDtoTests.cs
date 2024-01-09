namespace EngineBay.Persistence.Tests
{
    using System;
    using Xunit;

    public class BaseDtoTests
    {
        [Fact]
        public void ConstructorSetsProperties()
        {
            // arrange
            var id = Guid.NewGuid();
            var createdAt = DateTime.Now;
            var lastUpdatedAt = DateTime.Now;
            var model = new MockModel()
            {
                Id = id,
                CreatedAt = createdAt,
                LastUpdatedAt = lastUpdatedAt,
            };

            // act
            var baseDto = new BaseDto(model);

            // assert
            Assert.Equal(id, baseDto.Id);
            Assert.Equal(createdAt, baseDto.CreatedAt);
            Assert.Equal(lastUpdatedAt, baseDto.LastUpdatedAt);
        }
    }
}