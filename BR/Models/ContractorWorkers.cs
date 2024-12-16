using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BR.Models
{
    public class ContractorWorkers : TraceEntity
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string? Occupation { get; set; } //الحرفة 
        public string? Description { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsConfirmed { get; set; }
        public long CityId { get; set; }
        public long DirectorateId { get; set; }
        public long NeighborhoodId { get; set; }
        public string? Image { get; set; }
        [DisplayName("Contractor Id")]
        public string SysUserId { get; set; }
        public virtual SysUser SysUser { get; set; }


    }
}
