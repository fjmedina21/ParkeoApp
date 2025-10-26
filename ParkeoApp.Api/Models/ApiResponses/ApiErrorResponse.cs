namespace ParkeoApp.Api.Models.ApiResponses
{
    public class ApiErrorResponse(int statusCode, string? errorType = null, object errormessage = null!) : BaseApiResponse(statusCode)
    {
        public string? ErrorType { get; set; } = errorType ?? DefaultMessage(statusCode);
    }
}
