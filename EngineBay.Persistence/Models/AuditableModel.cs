namespace EngineBay.Persistence
{
    using EngineBay.Core;

    public class AuditableModel : BaseModel
    {
        public string? CreatedById { get; set; }

        public virtual ApplicationUser? CreatedBy { get; set; }

        public string? LastUpdatedById { get; set; }

        public virtual ApplicationUser? LastUpdatedBy { get; set; }
    }
}