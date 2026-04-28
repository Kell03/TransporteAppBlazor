using Domain.Dto;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using TransporteWeb.Contracts;
using TransporteWeb.Utils;

namespace TransporteWeb.Services
{
    public class AuthService
    {

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public AuthService(HttpClient httpClient, IOptions<ApiSettings> settings)
        {
            _httpClient = httpClient;
            _baseUrl = settings.Value.BaseUrl;
        }
        public async Task<AuthResponse?> Login(AuthRequest request)
        {
            UsuarioDto user = new UsuarioDto();
            user.Email = request.Username;
            user.Password = request.Password;

            var result = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Usuario/login", user);
            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<AuthResponse>(content);
            }
            else
            {
                return null;
            }
        }
    }
}
