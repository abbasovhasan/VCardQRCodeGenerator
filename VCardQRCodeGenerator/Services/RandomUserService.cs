using Newtonsoft.Json.Linq;
using VCardQRCodeGenerator.Models;

namespace VCardQRCodeGenerator.Services
{
    public class RandomUserService
    {
        private readonly HttpClient _httpClient;

        public RandomUserService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<VCard> GetRandomUserAsync()
        {
            var response = await _httpClient.GetStringAsync("https://randomuser.me/api/");
            var data = JObject.Parse(response);
            var user = data["results"][0];

            return new VCard
            {
                Id = Guid.NewGuid(),
                Firstname = user["name"]["first"].ToString(),
                Surname = user["name"]["last"].ToString(),
                Email = user["email"].ToString(),
                Phone = user["phone"].ToString(),
                Country = user["location"]["country"].ToString(),
                City = user["location"]["city"].ToString()
            };
        }
    }
}
