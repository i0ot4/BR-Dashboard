using System.ComponentModel.DataAnnotations;

namespace BR.Models.VMs
{
    public class CreateUserVM
    {
        [Display(Name = "الهاتف")]
        [Required]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "رقم الهاتف يتكون من 9 ارقام")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress]
        [Display(Name = "الإيميل")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(6, ErrorMessage = "كلمة المرور على الاقل 6 ارقام وأحرف")]
        [Display(Name = "كلمة السر")]
        public string Password { get; set; } = string.Empty;
        [Compare("Password", ErrorMessage = "كلمة المرور غير متطابقة")]
        [Display(Name = "تأكيد كلمة السر")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required]
        [MinLength(3, ErrorMessage = "يرجى ادخال اسم صحيح ومكتمل - على الاقل 3 أحرف")]
        [Display(Name = "الاسم")]
        public string FullName { get; set; } = string.Empty;
        [Display(Name = "صورة")]
        public string? Image { get; set; }
        [Display(Name = "نوع المستخدم")]
        public string? UserType { get; set; }
    }
}
