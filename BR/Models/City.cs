using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BR.Models
{
    public class City : TraceEntity
    {
        public long Id { get; set; }
        [Display(Name = "اسم المحافظة")]
        public string CityName { get; set; }
      
    }
}
