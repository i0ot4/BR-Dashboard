namespace BR.Models.VMs
{
    public class UserViewVM
    {
        public UserViewVM()
        {
            User = new SysUser();
        }
        public SysUser User { get; set; }
    }
}
