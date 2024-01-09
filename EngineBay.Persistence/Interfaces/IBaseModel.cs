namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public interface IBaseModel
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public static abstract void CreateDataAnnotations(ModelBuilder modelBuilder);
    }
}