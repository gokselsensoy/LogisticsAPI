namespace Application.Shared.ResultModels
{
    public interface IResult
    {
        bool Succeeded { get; }
        string Message { get; }
        List<string> Errors { get; }
    }

    // 2. Data Dönen Result (Query'ler ve ID dönen Command'lar için)
    public class Result<T> : IResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }

        public Result() { } // Serialization için boş constructor

        public static Result<T> Success(T data, string message = "İşlem başarılı.")
        {
            return new Result<T>
            {
                Succeeded = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>
            {
                Succeeded = false,
                Message = error,
                Errors = new List<string> { error },
                Data = default
            };
        }

        public static Result<T> Failure(List<string> errors)
        {
            return new Result<T>
            {
                Succeeded = false,
                Message = "Bir veya daha fazla hata oluştu.",
                Errors = errors,
                Data = default
            };
        }
    }

    // 3. Void Result (Update/Delete Command'ları için)
    public class Result : IResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public Result() { }

        public static Result Success(string message = "İşlem başarılı.")
        {
            return new Result
            {
                Succeeded = true,
                Message = message,
                Errors = null
            };
        }

        public static Result Failure(string error)
        {
            return new Result
            {
                Succeeded = false,
                Message = error,
                Errors = new List<string> { error }
            };
        }

        public static Result Failure(List<string> errors)
        {
            return new Result
            {
                Succeeded = false,
                Message = "Bir veya daha fazla hata oluştu.",
                Errors = errors
            };
        }
    }
}
