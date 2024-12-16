using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BR.Models.VMs
{
    public class LoginVM
    {
        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "رقم الهاتف يتكون من 9 ارقام")]
        [DisplayName("رقم الهاتف")]
        public string PhoneNumber { get; set; } = null!;
        [Required(ErrorMessage = "كلمة السر مطلوب")]
        [MinLength(6, ErrorMessage = "كلمة المرور على الاقل 6 ارقام وأحرف")]
        [DisplayName("كلمة السر")]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; } = false;
        public string? DeviceId { get; set; }
    }
}
