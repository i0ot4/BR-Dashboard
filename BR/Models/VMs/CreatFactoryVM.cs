using System.ComponentModel.DataAnnotations;

public class CreatFactoryVM
    {
        [Display(Name = "صورة")]
        public string? Image { get; set; }
        [Display(Name = "وصف")]
        public string? Description { get; set; }
        [Display(Name = "المحافظة")]
        public long CityId { get; set; }
        [Display(Name = "المديرية")]
        public long DirectorateId { get; set; }
        [Display(Name = "الحي")]
        public long NeighborhoodId { get; set; }
    }
