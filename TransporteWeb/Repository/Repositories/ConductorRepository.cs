using Domain.Dto;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using TransporteWeb.Repository.Interfaz;
using TransporteWeb.Repository.Utils;

namespace TransporteWeb.Repository.Repositories
{
    public class ConductorRepository : IBaseRepository<Conductor, ConductorDto>
    {

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ConductorRepository(HttpClient httpClient, IOptions<ApiSettings> settings)
        {
            _httpClient = httpClient;
            _baseUrl = settings.Value.BaseUrl;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/Conductor/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<ConductorDto>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Conductor");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var itemsData = JsonSerializer.Deserialize<List<ConductorDto>>(json, options);
            return itemsData;
        }

        public async Task<ConductorDto> GetById(int id)
        {
            var item = await JsonSerializer.DeserializeAsync<ConductorDto>
             (await _httpClient.GetStreamAsync($"{_baseUrl}/Conductor/{id}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (item == null)
            {
                return null;
            }
            else
            {
                return item;
            }
        }

        public async Task<ConductorDto> SaveAsync(ConductorDto entity)
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/Conductor", new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var itemsData = JsonSerializer.Deserialize<ConductorDto>(json, options);
                return itemsData;
            }
            else
                return null;
        }

        public async Task<ConductorDto> UpdateAsync(ConductorDto entity)
        {
            var json =
            new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/Conductor/{entity.Id}", json);

            if (response.IsSuccessStatusCode)
                return await JsonSerializer.DeserializeAsync<ConductorDto>(await response.Content.ReadAsStreamAsync());
            else
                return null;
        }

        public Task<UploadResultDto> UploadExcelAsync(Stream fileStream, string fileName)
        {
            throw new NotImplementedException();
        }

    }
}
