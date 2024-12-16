namespace BR.helper
{
    public class Message
    {
        public int code { get; set; }
        public string message { get; set; }
    }
    public interface IResult
    {
        //List<string> Messages { get; set; }
        Message Status { get; set; }

        bool Succeeded { get; set; }
    }
    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }
}
