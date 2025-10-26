
using System.Net;

namespace ParkeoApp.Domain.ApiResponseModels
{
	public abstract class BaseApiResponse(int statusCode = (int)HttpStatusCode.OK, string? message = null)
	{
		public bool Success { get; set; } = statusCode < 400;
		public int StatusCode { get; } = statusCode;
		public string? Message { get; set; } = message ?? DefaultMessage(statusCode);


		protected static string DefaultMessage(int statusCode) => statusCode switch
		{
			(int)HttpStatusCode.OK => "The request was successful.",
			(int)HttpStatusCode.Created => "The request was successful and a new resource was created.",
			(int)HttpStatusCode.BadRequest => "The server could not understand the request due to invalid syntax.",
			(int)HttpStatusCode.Unauthorized => "Authentication is required to access this resource.",
			(int)HttpStatusCode.Forbidden => "The client does not have access rights to the content.",
			(int)HttpStatusCode.NotFound => "The server cannot find the requested resource.",
			(int)HttpStatusCode.InternalServerError => "The server was unable to complete your request. Please try again later. If this problem persists, please contact support.",
			_ => "HTTP Status Code not defined"
		};
	}
}
