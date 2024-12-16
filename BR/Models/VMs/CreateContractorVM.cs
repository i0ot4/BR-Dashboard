using System.ComponentModel.DataAnnotations;

namespace BR.Models.VMs
{
    public class CreateContractorVM
    {
        [Display(Name = "صورة")]
        public string? Image { get; set; }
        [Display(Name = "رقم السجل التجاري")]
        public string CommercialRegistrationNo { get; set; }
        [Display(Name = "صورة السجل التجاري")]
        public string? CommericalRegistrationImage { get; set; }
        [Display(Name = "وصف")]
        public string? Description { get; set; }
        [Display(Name = "المحافظة")]
        public long CityId { get; set; }
        [Display(Name = "المديرية")]
        public long DirectorateId { get; set; }
        [Display(Name = "الحي")]
        public long NeighborhoodId { get; set; }

    }
}
