using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BR.Models
{
    public class Directorate : TraceEntity
    {
        public long Id { get; set; }
        [Display(Name = "اسم المديرية")]
        public string DirectorateName { get; set; }
        [Display(Name = "المحافظة")]
        public long CityId { get; set; }
        virtual public City City { get; set; }

 

    }
}
