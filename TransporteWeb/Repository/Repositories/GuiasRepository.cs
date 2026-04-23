using Domain.Dto;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using TransporteWeb.Repository.Interfaz;
using TransporteWeb.Repository.Utils;

namespace TransporteWeb.Repository.Repositories
{
    public class GuiasRepository : IBaseRepository<Guia, GuiaDto>
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public GuiasRepository(HttpClient httpClient, IOptions<ApiSettings> settings)
        {
            _httpClient = httpClient;
            _baseUrl = settings.Value.BaseUrl;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/Guias/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<GuiaDto>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Guias");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var itemsData = JsonSerializer.Deserialize<List<GuiaDto>>(json, options);
            return itemsData;
        }

        public async Task<GuiaDto> GetById(int id)
        {
            var item = await JsonSerializer.DeserializeAsync<GuiaDto>
             (await _httpClient.GetStreamAsync($"{_baseUrl}/Guias/{id}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (item == null)
            {
                return null;
            }
            else
            {
                return item;
            }
        }

        public async Task<GuiaDto> SaveAsync(GuiaDto entity)
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/Guias", new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var itemsData = JsonSerializer.Deserialize<GuiaDto>(json, options);
                return itemsData;
            }
            else
                return null;
        }

        public async Task<GuiaDto> UpdateAsync(GuiaDto entity)
        {
            var json =
            new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/Guias/{entity.Id}", json);

            if (response.IsSuccessStatusCode)
                return await JsonSerializer.DeserializeAsync<GuiaDto>(await response.Content.ReadAsStreamAsync());
            else
                return null;
        }

        public Task<UploadResultDto> UploadExcelAsync(Stream fileStream, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
