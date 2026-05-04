using Blazored.SessionStorage;
using Domain.Dto;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TransporteApi.Models;
using TransporteWeb.Repository.Interfaz;
using TransporteWeb.Utils;

namespace TransporteWeb.Repository.Repositories
{
    public class RolRepository : IBaseRepository<Rol, RolDto>
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ISessionStorageService _sessionStorageService;

        public RolRepository(HttpClient httpClient, IOptions<ApiSettings> settings, ISessionStorageService sessionStorageService)
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

        public async Task<RolDto> SaveAsync(RolDto entity)
        {
            await AddTokenToHeaderAsync();
            var response = await _httpClient.PostAsync($"{_baseUrl}/Rol", new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var itemsData = JsonSerializer.Deserialize<RolDto>(json, options);
                return itemsData;
            }
            else
                return null;

        }

        public async Task<List<RolDto>> GetAllAsync()
        {
            await AddTokenToHeaderAsync();
            var response = await _httpClient.GetAsync($"{_baseUrl}/Rol");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var itemsData = JsonSerializer.Deserialize<List<RolDto>>(json, options);
            return itemsData;
        }

        public async Task<RolDto> GetById(int id)
        {
            await AddTokenToHeaderAsync();
            var item = await JsonSerializer.DeserializeAsync<RolDto>
              (await _httpClient.GetStreamAsync($"{_baseUrl}/Rol/{id}"),new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
           
            if (item == null)
            {
                return null;
            }
            else{
                return item;
            }

        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                await AddTokenToHeaderAsync();
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/Rol/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<RolDto> UpdateAsync(RolDto entity)
        {
            await AddTokenToHeaderAsync();
            var json =
         new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/Rol/{entity.Id}", json);

            if (response.IsSuccessStatusCode)
                return await JsonSerializer.DeserializeAsync<RolDto>(await response.Content.ReadAsStreamAsync());
            else
                return null;
        }

        public Task<UploadResultDto> UploadExcelAsync(Stream fileStream, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExportExcelAsync()
        {
            throw new NotImplementedException();
        }

        Task<Stream> IBaseRepository<Rol, RolDto>.ExportExcelAsync(ExportRequest exportRequest)
        {
            throw new NotImplementedException();
        }
    }
}
