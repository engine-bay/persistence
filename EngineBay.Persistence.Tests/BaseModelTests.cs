namespace EngineBay.Persistence.Tests
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class BaseModelTests
    {
        [Fact]
        public void NewObjectSetsProperties()
        {
            // arrange
            var id = Guid.NewGuid();
            var createdAt = DateTime.Now;
            var lastUpdatedAt = DateTime.Now;

            // act
            var baseModel = new MockModel()
            {
                Id = id,
                CreatedAt = createdAt,
                LastUpdatedAt = lastUpdatedAt,
            };

            // assert
            Assert.Equal(id, baseModel.Id);
            Assert.Equal(createdAt, baseModel.CreatedAt);
            Assert.Equal(lastUpdatedAt, baseModel.LastUpdatedAt);
        }

        [Fact]
        public void CreateDataAnnotationsThrowsException()
        {
            // arrange
            ModelBuilder modelBuilder = new ModelBuilder();

            // act
            // assert
            Assert.Throws<NotImplementedException>(() => BaseModel.CreateDataAnnotations(modelBuilder));
        }
    }
}