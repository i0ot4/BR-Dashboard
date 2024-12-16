using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BR.Models
{
    public class Neighborhood : TraceEntity
    {
        public long Id { get; set; }
        [Display(Name = "اسم الحي")]
        public string NeighborhoodName { get; set; }
        [Display(Name = "المديرية")]
        public long DirectorateId { get; set; }
        virtual public Directorate Directorate { get; set; }
        
    }
}
