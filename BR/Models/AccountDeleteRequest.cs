using System.ComponentModel;

namespace BR.Models
{
    public class AccountDeleteRequest : TraceEntity
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string? PeasonOfDelete { get; set; }
        public int State { get; set; }
        virtual public SysUser SysUser { get; set; }

    }
}
