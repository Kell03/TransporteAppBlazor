using Blazored.SessionStorage;
using Domain.Dto;
using Domain.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TransporteWeb.Repository.Interfaz;
using TransporteWeb.Utils;

namespace TransporteWeb.Repository.Repositories
{
    public class ClienteRepository : IBaseRepository<Cliente, ClienteDto>
    {

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ISessionStorageService _sessionStorageService;

        public ClienteRepository(HttpClient httpClient, IOptions<ApiSettings> settings, ISessionStorageService sessionStorageService)
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

                var response = await _httpClient.DeleteAsync($"{_baseUrl}/Cliente/{id}");
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

        public async Task<List<ClienteDto>> GetAllAsync()
        {
            await AddTokenToHeaderAsync();

            var response = await _httpClient.GetAsync($"{_baseUrl}/Cliente");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var itemsData = JsonSerializer.Deserialize<List<ClienteDto>>(json, options);
            return itemsData;
        }

        public async Task<ClienteDto> GetById(int id)
        {
            await AddTokenToHeaderAsync();

            var item = await JsonSerializer.DeserializeAsync<ClienteDto>
              (await _httpClient.GetStreamAsync($"{_baseUrl}/Cliente/{id}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (item == null)
            {
                return null;
            }
            else
            {
                return item;
            }
        }

        public async Task<ClienteDto> SaveAsync(ClienteDto entity)
        {
            await AddTokenToHeaderAsync();
            var response = await _httpClient.PostAsync($"{_baseUrl}/Cliente", new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var itemsData = JsonSerializer.Deserialize<ClienteDto>(json, options);
                return itemsData;
            }
            else
                return null;
        }

        public async Task<ClienteDto> UpdateAsync(ClienteDto entity)
        {

            await AddTokenToHeaderAsync();
            var json = new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/Cliente/{entity.Id}", json);

            if (response.IsSuccessStatusCode)
                return await JsonSerializer.DeserializeAsync<ClienteDto>(await response.Content.ReadAsStreamAsync());
            else
                return null;
        }

        public Task<UploadResultDto> UploadExcelAsync(Stream fileStream, string fileName)
        {
            throw new NotImplementedException();
        }

        Task<Stream> IBaseRepository<Cliente, ClienteDto>.ExportExcelAsync(ExportRequest exportRequest)
        {
            throw new NotImplementedException();
        }
    }
}
