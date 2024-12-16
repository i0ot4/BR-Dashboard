namespace BR.Models.VMs
{
    public class AuthVM
    {
        public SysUser? User { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
