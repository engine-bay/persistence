namespace EngineBay.Persistence
{
    public class BaseDto
    {
        public BaseDto(BaseModel baseModel)
        {
            ArgumentNullException.ThrowIfNull(baseModel);

            this.Id = baseModel.Id;
            this.CreatedAt = baseModel.CreatedAt;
            this.LastUpdatedAt = baseModel.LastUpdatedAt;
        }

        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }
    }
}