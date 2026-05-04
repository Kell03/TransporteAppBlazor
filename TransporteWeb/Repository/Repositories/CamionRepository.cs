using Blazored.SessionStorage;
using Domain.Dto;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TransporteWeb.Repository.Interfaz;
using TransporteWeb.Utils;

namespace TransporteWeb.Repository.Repositories
{
    public class CamionRepository : IBaseRepository<Camion, CamionDto>
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ISessionStorageService _sessionStorageService;
        public CamionRepository(HttpClient httpClient, IOptions<ApiSettings> settings, ISessionStorageService sessionStorageService)
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
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/Camion/{id}");
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

        public async Task<List<CamionDto>> GetAllAsync()
        {

            try
            {
                await AddTokenToHeaderAsync();
                var response = await _httpClient.GetAsync($"{_baseUrl}/Camion");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var itemsData = JsonSerializer.Deserialize<List<CamionDto>>(json, options);
                return itemsData;

            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        public async Task<CamionDto> GetById(int id)
        {
            await AddTokenToHeaderAsync();
            var item = await JsonSerializer.DeserializeAsync<CamionDto>
             (await _httpClient.GetStreamAsync($"{_baseUrl}/Camion/{id}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (item == null)
            {
                return null;
            }
            else
            {
                return item;
            }
        }

        public async Task<CamionDto> SaveAsync(CamionDto entity)
        {
            await AddTokenToHeaderAsync();
            var response = await _httpClient.PostAsync($"{_baseUrl}/Camion", new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var itemsData = JsonSerializer.Deserialize<CamionDto>(json, options);
                return itemsData;
            }
            else
                return null;
        }

        public async Task<CamionDto> UpdateAsync(CamionDto entity)
        {
            await AddTokenToHeaderAsync();
            var json =
            new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/Camion/{entity.Id}", json);

            if (response.IsSuccessStatusCode)
                return await JsonSerializer.DeserializeAsync<CamionDto>(await response.Content.ReadAsStreamAsync());
            else
                return null;
        }

        public virtual async Task<UploadResultDto> UploadExcelAsync(Stream fileStream, string fileName)
        {
            await AddTokenToHeaderAsync();
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync($"{_baseUrl}/Camion/upload", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<UploadResultDto>(json, options);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al subir archivo: {response.StatusCode} - {error}");
            }
        }

        Task<Stream> IBaseRepository<Camion, CamionDto>.ExportExcelAsync(ExportRequest exportRequest)
        {
            throw new NotImplementedException();
        }
    }
}
