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

        public async Task<UploadResultDto> UploadExcelAsync(Stream fileStream, string fileName)
        {
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync($"{_baseUrl}/Guias/upload", content);

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

        public async Task<Stream> ExportExcelAsync(ExportRequest exportRequest)
        {
            try
            {
                // ✅ Ver el JSON que se está enviando

                var response = await _httpClient.PostAsync($"{_baseUrl}/Guias/export/excel", new StringContent(JsonSerializer.Serialize(exportRequest), Encoding.UTF8, "application/json"));


                // ✅ Leer el error específico
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"ERROR RESPONSE: {errorContent}");

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ExportarGuiasExcelStreamAsync: {ex.Message}");
                throw;
            }
        }
    }
}
