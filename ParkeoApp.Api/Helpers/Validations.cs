using ParkeoApp.Api.Data;
using ParkeoApp.Api.Models.ApiResponses;
using ParkeoApp.Api.Models.ThirdPartiesModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ParkeoApp.Api.Helpers
{
    public static class Validations
    {
        public async static Task<ApiResponse<CedulaValidationResponse>> ValidateCedula(string cedula)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync($"https://api.digital.gob.do/v3/cedulas/{cedula}/validate");

            response.EnsureSuccessStatusCode();

            string data = await response.Content.ReadAsStringAsync();
            var deserializedObject = JsonConvert.DeserializeObject<CedulaValidationResponse>(data)!;

            if (!deserializedObject.Valid) return new ApiResponse<CedulaValidationResponse>(statusCode: (int)response.StatusCode, message: deserializedObject.Message);

            return new ApiResponse<CedulaValidationResponse>();
        }

        public async static Task<bool> EmailExist(string email, ParkeoAppContext context)
        {
            return await context.users.AnyAsync(e => e.email == email);
        }


    }
}
