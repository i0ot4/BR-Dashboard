using System.ComponentModel;

namespace BR.Models
{
    public abstract class TraceEntity
    {
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
}
