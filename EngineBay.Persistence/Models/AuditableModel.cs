namespace EngineBay.Persistence
{
    using EngineBay.Core;

    public class AuditableModel : BaseModel
    {
        public Guid? CreatedById { get; set; }

        public virtual ApplicationUser? CreatedBy { get; set; }

        public Guid? LastUpdatedById { get; set; }

        public virtual ApplicationUser? LastUpdatedBy { get; set; }
    }
}