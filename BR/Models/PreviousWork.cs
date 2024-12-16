using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BR.Models
{
    public class PreviousWork : TraceEntity
    {
        public long Id { get; set; }
        [Display(Name = "اسم العمل")]
        public string Title { get; set; }
        [Display(Name = "وصف")]
        public string Details { get; set; }
        [Display(Name = "المستخدم")]
        public string UserId { get; set; }
        virtual public SysUser SysUser { get; set; }
        public List<PreviousWorkImage>? PreviousWorkImages { get; set; }




    }
}
