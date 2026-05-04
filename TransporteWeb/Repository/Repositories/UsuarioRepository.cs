using Blazored.SessionStorage;
using Domain.Dto;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TransporteWeb.Repository.Interfaz;
using TransporteWeb.Utils;

namespace TransporteWeb.Repository.Repositories
{
    public class UsuarioRepository : IBaseRepository<Usuario, UsuarioDto>
    {

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ISessionStorageService _sessionStorageService;

        public UsuarioRepository(HttpClient httpClient, IOptions<ApiSettings> settings, ISessionStorageService sessionStorageService)
        {
            _httpClient = httpClient;
            _baseUrl = settings.Value.BaseUrl;
            _sessionStorageService = sessionStorageService;
        }


        private async Task AddTokenToHeaderAsync()
        {
            var tokenResult = await _sessionStorageService.GetItemAsync<string>("token");

            if (!string.IsNullOrEmpty(tokenResult))
            {
                var token = tokenResult.Trim().Replace("\"", "");
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", token);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                await AddTokenToHeaderAsync();

                var response = await _httpClient.DeleteAsync($"{_baseUrl}/Usuario/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> ExportExcelAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<UsuarioDto>> GetAllAsync()
        {
            await AddTokenToHeaderAsync();

            var response = await _httpClient.GetAsync($"{_baseUrl}/Usuario");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var itemsData = JsonSerializer.Deserialize<List<UsuarioDto>>(json, options);
            return itemsData;
        }

        public async Task<UsuarioDto> GetById(int id)
        {
            await AddTokenToHeaderAsync();

            var item = await JsonSerializer.DeserializeAsync<UsuarioDto>
              (await _httpClient.GetStreamAsync($"{_baseUrl}/Usuario/{id}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (item == null)
            {
                return null;
            }
            else
            {
                return item;
            }
        }

        public async Task<UsuarioDto> SaveAsync(UsuarioDto entity)
        {
            await AddTokenToHeaderAsync();
            var response = await _httpClient.PostAsync($"{_baseUrl}/Usuario", new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var itemsData = JsonSerializer.Deserialize<UsuarioDto>(json, options);
                return itemsData;
            }
            else
                return null;
        }

        public async Task<UsuarioDto> UpdateAsync(UsuarioDto entity)
        {

            await AddTokenToHeaderAsync();
            var json = new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/Usuario/{entity.Id}", json);

            if (response.IsSuccessStatusCode)
                return await JsonSerializer.DeserializeAsync<UsuarioDto>(await response.Content.ReadAsStreamAsync());
            else
                return null;
        }

        public Task<UploadResultDto> UploadExcelAsync(Stream fileStream, string fileName)
        {
            throw new NotImplementedException();
        }

        Task<Stream> IBaseRepository<Usuario, UsuarioDto>.ExportExcelAsync(ExportRequest exportRequest)
        {
            throw new NotImplementedException();
        }
    }
}
