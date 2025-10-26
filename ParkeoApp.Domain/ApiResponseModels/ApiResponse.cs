namespace ParkeoApp.Domain.ApiResponseModels
{
    public class ApiResponse<T>(int statusCode = 200, string? message = null, List<T> data = null! ) : BaseApiResponse(statusCode, message)
    {
        public List<T> Data { get; set; } = data;
    }

    public class ApiResponse(int statusCode = 200, string? message = null) : BaseApiResponse(statusCode, message)
    {

    }
}
