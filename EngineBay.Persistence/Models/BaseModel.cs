namespace EngineBay.Persistence
{
    using System;
    using Microsoft.EntityFrameworkCore;

    public abstract class BaseModel : IBaseModel
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public static void CreateDataAnnotations(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }
    }
}