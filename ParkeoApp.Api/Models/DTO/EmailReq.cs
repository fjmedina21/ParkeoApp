namespace ParkeoApp.Api.Models.DTO
{
	public record EmailReq(string[] To, string Subject, string Body);
}
