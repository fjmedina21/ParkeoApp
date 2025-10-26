namespace ParkeoApp.Domain.DTO
{
	public record EmailReq(string[] To, string Subject, string Body);
}
