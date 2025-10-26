
namespace ParkeoApp.Api.Models.ApiResponses
{
	public abstract class BaseApiResponse(int statusCode = StatusCodes.Status200OK, string? message = null)
	{
		public bool Success { get; set; } = statusCode < 400;
		public int StatusCode { get; } = statusCode;
		public string? Message { get; set; } = message ?? DefaultMessage(statusCode);


		protected static string DefaultMessage(int statusCode) => statusCode switch
		{
			StatusCodes.Status100Continue => "The client should continue with its request.",
			StatusCodes.Status101SwitchingProtocols => "Switching protocols - the server is changing the protocol.",
			StatusCodes.Status200OK => "The request was successful.",
			StatusCodes.Status201Created => "The request was successful and a new resource was created.",
			StatusCodes.Status202Accepted => "The request has been accepted for processing, but not completed.",
			StatusCodes.Status203NonAuthoritative => "Returned metadata may not be from the origin server.",
			StatusCodes.Status204NoContent => "The server successfully processed the request and is not returning any content.",
			StatusCodes.Status205ResetContent => "The server successfully processed the request, but asks that the client reset its document view.",
			StatusCodes.Status206PartialContent => "The server is delivering only part of the resource due to a range header sent by the client.",
			StatusCodes.Status300MultipleChoices => "There are multiple options for the resource.",
			StatusCodes.Status301MovedPermanently => "The resource has been moved to a new permanent URI.",
			StatusCodes.Status302Found => "The requested resource resides temporarily under a different URI.",
			StatusCodes.Status303SeeOther => "The response can be found under another URI using a GET method.",
			StatusCodes.Status304NotModified => "The resource has not been modified since the last request.",
			StatusCodes.Status305UseProxy => "The requested resource must be accessed through the proxy given by the Location field.",
			StatusCodes.Status307TemporaryRedirect => "The resource resides temporarily under a different URI.",
			StatusCodes.Status308PermanentRedirect => "The resource has been permanently moved to a new URI.",
			StatusCodes.Status400BadRequest => "The server could not understand the request due to invalid syntax.",
			StatusCodes.Status401Unauthorized => "Authentication is required to access this resource.",
			StatusCodes.Status402PaymentRequired => "Reserved for future use; used in digital payment systems.",
			StatusCodes.Status403Forbidden => "The client does not have access rights to the content.",
			StatusCodes.Status404NotFound => "The server cannot find the requested resource.",
			StatusCodes.Status405MethodNotAllowed => "The method specified is not allowed for the resource.",
			StatusCodes.Status406NotAcceptable => "The server cannot produce a response matching the list of acceptable values.",
			StatusCodes.Status407ProxyAuthenticationRequired => "The client must first authenticate with the proxy.",
			StatusCodes.Status408RequestTimeout => "The server timed out waiting for the request.",
			StatusCodes.Status409Conflict => "The request could not be processed because of a conflict.",
			StatusCodes.Status410Gone => "The resource requested is no longer available.",
			StatusCodes.Status411LengthRequired => "The server refuses to accept the request without a defined content length.",
			StatusCodes.Status412PreconditionFailed => "The server does not meet one of the preconditions.",
			StatusCodes.Status413PayloadTooLarge => "The request entity is larger than the server is willing to process.",
			StatusCodes.Status414UriTooLong => "The URI provided was too long for the server to process.",
			StatusCodes.Status415UnsupportedMediaType => "The server does not support the media format.",
			StatusCodes.Status416RangeNotSatisfiable => "The range specified cannot be satisfied.",
			StatusCodes.Status417ExpectationFailed => "The server cannot meet the requirements of the Expect header.",
			StatusCodes.Status422UnprocessableEntity => "The request was well-formed but could not be processed.",
			StatusCodes.Status428PreconditionRequired => "The server requires the request to be conditional.",
			StatusCodes.Status429TooManyRequests => "The user has sent too many requests in a given amount of time.",
			StatusCodes.Status431RequestHeaderFieldsTooLarge => "The server refuses to process the request because the header fields are too large.",
			StatusCodes.Status451UnavailableForLegalReasons => "The resource is unavailable due to legal reasons.",
			StatusCodes.Status500InternalServerError => "The server was unable to complete your request. Please try again later. If this problem persists, please contact support.",
			StatusCodes.Status501NotImplemented => "The server does not support the requested functionality.",
			StatusCodes.Status502BadGateway => "The server received an invalid response from the upstream server.",
			StatusCodes.Status503ServiceUnavailable => "The server is currently unable to handle the request.",
			StatusCodes.Status504GatewayTimeout => "The server did not receive a timely response from an upstream server.",
			StatusCodes.Status505HttpVersionNotsupported => "The server does not support the HTTP protocol version.",
			_ => "HTTP Status Code not defined"
		};
	}
}
