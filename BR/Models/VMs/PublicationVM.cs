using System.ComponentModel.DataAnnotations;

namespace BR.Models.VMs
{
    public class PublicationVM
    {
        public long Id { get; set; }
        [Display(Name = "العنوان")]
        public string Title { get; set; }
        [Display(Name = "المحتوى")]
        public string Content { get; set; }
        [Display(Name = "رقم التواصل")]
        public string PhoneNumber { get; set; }
        [Display(Name = "المستخدم")]
        public string UserId { get; set; }
    }
}
