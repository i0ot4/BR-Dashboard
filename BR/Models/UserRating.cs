using System.ComponentModel;

namespace BR.Models
{
    public class UserRating : TraceEntity
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        virtual public SysUser SysUser { get; set; }

    }
}