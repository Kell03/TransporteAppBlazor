using Domain.Dto;
using Microsoft.Extensions.Options;
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

        public RolRepository(HttpClient httpClient, IOptions<ApiSettings> settings)
        {
            _httpClient = httpClient;
            _baseUrl = settings.Value.BaseUrl;
        }




        public async Task<RolDto> SaveAsync(RolDto entity)
        {
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
