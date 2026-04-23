using Domain.Dto;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using TransporteWeb.Repository.Interfaz;
using TransporteWeb.Repository.Utils;

namespace TransporteWeb.Repository.Repositories
{
    public class CdRepository : IBaseRepository<CentroDistribucion, CentroDistribucionDto>
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public CdRepository(HttpClient httpClient, IOptions<ApiSettings> settings)
        {
            _httpClient = httpClient;
            _baseUrl = settings.Value.BaseUrl;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/CentroDistribucion/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CentroDistribucionDto>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/CentroDistribucion");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var itemsData = JsonSerializer.Deserialize<List<CentroDistribucionDto>>(json, options);
            return itemsData;
        }

        public async Task<CentroDistribucionDto> GetById(int id)
        {
            var item = await JsonSerializer.DeserializeAsync<CentroDistribucionDto>
             (await _httpClient.GetStreamAsync($"{_baseUrl}/CentroDistribucion/{id}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (item == null)
            {
                return null;
            }
            else
            {
                return item;
            }
        }

        public async Task<CentroDistribucionDto> SaveAsync(CentroDistribucionDto entity)
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/CentroDistribucion", new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var itemsData = JsonSerializer.Deserialize<CentroDistribucionDto>(json, options);
                return itemsData;
            }
            else
                return null;
        }

        public async Task<CentroDistribucionDto> UpdateAsync(CentroDistribucionDto entity)
        {
            var json =
            new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/CentroDistribucion/{entity.Id}", json);

            if (response.IsSuccessStatusCode)
                return await JsonSerializer.DeserializeAsync<CentroDistribucionDto>(await response.Content.ReadAsStreamAsync());
            else
                return null;
        }

        public async Task<UploadResultDto> UploadExcelAsync(Stream fileStream, string fileName)
        {
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync($"{_baseUrl}/CentroDistribucion/upload", content);

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

    }
}
