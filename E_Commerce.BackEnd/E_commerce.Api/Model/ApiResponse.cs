
namespace E_commerce.Api.Model
{
    public class ApiResponse<T>
    {
        public bool Success {get; set;}
        public string? Message {get; set;}
        public T? Result {get; set;}
        public ErrorDetails Error {get; set;}
        public MetaData Meta {get; set;} = new MetaData();
    }

    public class ErrorDetails{
        public string Code {get; set;}
        public string Details {get; set;}
        public Dictionary<string, string[]> ValidationErrors {get; set;}
    }

    public class MetaData{
        public int StatusCode {get; set;} = 200;
        public string RequestId {get; set;} = Guid.NewGuid().ToString();
        public string version {get; set;} = "1.0";
        public DateTime Timestamp {get; set;} = DateTime.UtcNow;
        public PaginationInfo Pagination {get; set;}
    }

    public class PaginationInfo{
        public int TotalItems {get; set;}
        public int CurrentPage {get; set;}
        public int PageSize {get; set;}
        public int TotalPages {get; set;}
        public bool HasPrevious {get; set;}
        public bool HasNext {get; set;}
    }
}