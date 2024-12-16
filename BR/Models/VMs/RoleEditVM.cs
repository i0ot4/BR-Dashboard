using System.ComponentModel.DataAnnotations;

namespace BR.Models.VMs
{
    public class RoleVM
    {
        public string Name { get; set; }
    }
    public class RoleEditVM
    {
        public RoleEditVM()
        {
            Users = new List<string>();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
