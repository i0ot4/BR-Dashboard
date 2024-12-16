using System.ComponentModel.DataAnnotations;

namespace BR.Models.VMs
{
    public class CreateRoleVM
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}
