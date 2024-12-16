using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace BR.Models
{
    public class SysUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? CommercialRegistrationNo { get; set; } //السجل التجاري فقط للنوع المقاولين والمحلات
        public string? CommericalRegistrationImage { get; set; } //صورة السجل التجاري
        public string? Occupation { get; set; } //الحرفة وهي فقط خاصة بالنوع العمال
        public string? Description { get; set; }
        public bool IsConfirmed { get; set; }
        public long CityId { get; set; }
        public long DirectorateId { get; set; }
        public long NeighborhoodId { get; set; }
        public string? Image { get; set; }
        public string? UserType { get; set; }//عامل - مقاول - محل بناء - ...
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        [DefaultValue(false)]
        public bool IsActive { get; set; }
        public bool CanEdit { get; set; } = false;
        public int EditCount { get; set; } = 0;
        public List<Publication>? Publications {  get; set; }
        public List<PreviousWork>? PreviousWork {  get; set; }
    }
}
