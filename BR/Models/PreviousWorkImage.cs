using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BR.Models
{
    public class PreviousWorkImage : TraceEntity
    {
        public long Id { get; set; }
        [Display(Name = "عنوان الصورة")]
        public string ImageTitle { get; set; }
        [Display(Name = "صورة")]
        public string? ImagePath { get; set; }
        [Display(Name = "العمل السابق")]
        public long PreviousWorkId { get; set; }
        virtual public PreviousWork? PreviousWork { get; set; }

      



    }
}
