namespace ParkeoApp.Application.Helpers
{
	public static class DominicanRepublicTime
	{
		private static readonly TimeZoneInfo TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("America/Santo_Domingo");
		public static DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo);

		public static DateTime ConvertLocalToUtc(DateTime localTime)
		{
			localTime = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);
			return TimeZoneInfo.ConvertTimeToUtc(localTime, TimeZoneInfo);
		}

		public static DateTime ConvertUtcToLocal(DateTime utcTime)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo);
		}
	}
}
