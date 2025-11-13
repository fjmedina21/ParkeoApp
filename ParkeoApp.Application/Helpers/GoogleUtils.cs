using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ParkeoApp.Domain.DTO;

namespace ParkeoApp.Application.Helpers
{
	public static class GoogleUtils
	{
		public async static Task<List<GetParkingLot>> GetLotsOrderedByDistanceAsync(
			double userLat,
			double userLng,
			List<GetParkingLot> parkingLots,
			IConfiguration configuration)
		{
			string? apiKey = configuration.GetValue<string>("GoogleMaps:ApiKey");
			if (parkingLots.Count == 0) return [];

			const string url = "https://routes.googleapis.com/distanceMatrix/v2:computeRouteMatrix";

			var destinations = parkingLots.Select(l => new
			{
				waypoint = new
				{
					location = new
					{
						latLng = new { latitude = l.Latitude, longitude = l.Longitude }
					}
				}
			}).ToList();

			var body = new
			{
				origins = new[]
				{
					new
					{
						waypoint = new
						{
							location = new
							{
								latLng = new { latitude = userLat, longitude = userLng }
							}
						}
					}
				},
				destinations = destinations,
				travelMode = "DRIVE",
				routingPreference = "TRAFFIC_AWARE",
				units = "METRIC"
			};

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);
			client.DefaultRequestHeaders.Add("X-Goog-FieldMask", "originIndex,destinationIndex,distanceMeters");

			string jsonBody = JsonSerializer.Serialize(body, new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			});

			var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PostAsync(url, content);

			string responseText = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine("Google API Error: " + responseText);
				return parkingLots;
			}

			var distances = new List<(Guid LotId, double DistanceMeters)>();
			using var doc = JsonDocument.Parse(responseText);

			foreach (var element in doc.RootElement.EnumerateArray())
			{
				int destIndex = element.GetProperty("destinationIndex").GetInt32();
				distances.Add(
					element.TryGetProperty("distanceMeters", out JsonElement distance)
						? (parkingLots[destIndex].ParkingLotId, distance.GetDouble())
						: (parkingLots[destIndex].ParkingLotId, double.MaxValue));
			}

			// Asignamos DistanceKm a cada lote
			foreach ((Guid lotId, double distanceMeters) in distances)
			{
				GetParkingLot? lot = parkingLots.FirstOrDefault(l => l.ParkingLotId == lotId);
				if (lot != null) lot.DistanceKm = Math.Round(distanceMeters / 1000.0, 1);
			}

			// Ordenamos los lotes por distancia
			return parkingLots.OrderBy(l => l.DistanceKm).ToList();
		}
	}
}
