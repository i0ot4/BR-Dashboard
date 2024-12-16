namespace BR.Models.VMs
{
    public class HomeVM
    {
        public int Users { get; set; }
        public int Contractors { get; set; }
        public int Workers { get; set; }
        public int Shops { get; set; }
        public List<SysUser> sysUsers { get; set; }
    }
}
