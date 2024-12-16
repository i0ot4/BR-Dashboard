namespace BR.Models
{
    public class AccountEditRequest : TraceEntity
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Details { get; set; }
        public bool IsConfirmed { get; set; }

    }
}
