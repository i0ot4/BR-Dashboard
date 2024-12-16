namespace BR.helper
{
    public class Result : IResult
    {
        public Result()
        {

        }
        public Message Status { get; set; }

        public bool Succeeded { get; set; }
        public static IResult Fail()
        {
            return new Result { Succeeded = false };
        }

        public static IResult Fail(string message, int code)
        {
            return new Result { Succeeded = false, Status = new Message { code = code, message = message } };
        }
        public static Task<IResult> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public static Task<IResult> FailAsync(string message, int code = 500)
        {
            return Task.FromResult(Fail(message, code));
        }
        public static IResult Success()
        {
            return new Result { Succeeded = true };
        }

        public static IResult Success(string message, int code)
        {
            return new Result { Succeeded = true, Status = new Message { code = code, message = message } };
        }
        public static Task<IResult> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public static Task<IResult> SuccessAsync(string message, int code = 200)
        {
            return Task.FromResult(Success(message, code));
        }
    }
    public class Result<T> : IResult<T>
    {
        public Result()
        {
        }

        public T Data { get; set; }
        public Message Status { get; set; }
        public bool Succeeded { get; set; }

        public static Result<T> Fail()
        {
            return new Result<T> { Succeeded = false };
        }

        public static Result<T> Fail(string message)
        {
            return new Result<T> { Succeeded = false, Status = new Message { code = 500, message = message } };
        }

        public static Result<T> Fail(string message, int code)
        {
            return new Result<T> { Succeeded = false, Status = new Message { code = code, message = message } };
        }

        public static Task<Result<T>> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public static Task<Result<T>> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public static Task<Result<T>> FailAsync(string message, int code)
        {
            return Task.FromResult(Fail(message, code));
        }

        public static Result<T> Success()
        {
            return new Result<T> { Succeeded = true };
        }

        public static Result<T> Success(string message, int code)
        {
            return new Result<T> { Succeeded = true, Status = new Message { code = code, message = message } };
        }

        public static Result<T> Success(T data)
        {
            return new Result<T> { Succeeded = true, Data = data };
        }

        public static Result<T> Success(T data, string message, int code)
        {
            return new Result<T> { Succeeded = true, Data = data, Status = new Message { code = code, message = message } };
        }

        public static Task<Result<T>> SuccessAsync(string message, int code = 200)
        {
            return Task.FromResult(Success(message, code));
        }

        public static Task<Result<T>> SuccessAsync(T data, string v)
        {
            return Task.FromResult(Success(data));
        }

        public static Task<Result<T>> SuccessAsync(T data, string message, int code = 200)
        {
            return Task.FromResult(Success(data, message, code));
        }
    }
}
