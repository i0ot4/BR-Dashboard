namespace BR.Models.VMs
{
    public class PreviousWorkImageVM
    {
        public IFormFile Image { get; set; }
        public IFormFile? Image2 { get; set; }
        public IFormFile? Image3 { get; set; }
        public string UserId { get; set; }
        public long PrevoiusWorkId { get; set; }

    }
}
