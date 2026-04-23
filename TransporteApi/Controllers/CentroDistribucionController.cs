using AutoMapper;
using ClosedXML.Excel;
using Domain.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransporteApi.Models;
using TransporteApi.Services;

namespace TransporteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentroDistribucionController : ControllerBase
    {

        protected readonly CentroDistribucionService _service;
        protected readonly IMapper _mapper;

        public CentroDistribucionController(CentroDistribucionService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<CentroDistribucionDto> lista = await _service.GetAllAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            CentroDistribucionDto item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CentroDistribucionDto itemDto)
        {
            try
            {
                CentroDistribucion item = _mapper.Map<CentroDistribucion>(itemDto);

                itemDto = await _service.CreateAsync(item);
                return Ok(itemDto);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CentroDistribucionDto itemDto)
        {

            CentroDistribucion itemExistente = await _service.FindAsync(id);
            if (itemExistente == null)
                return NotFound();

            _mapper.Map(itemDto, itemExistente);


            var resultado = await _service.UpdateAsync(itemExistente);

            return Ok(resultado);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {

            return Ok(await _service.DeleteAsync(id));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                var resultado = new UploadResultDto();


                if (file == null || file.Length == 0)
                {
                    resultado.Message = "No se ha seleccionado ningún archivo";
                    return BadRequest(resultado);
                }

                // Verificar que sea un archivo Excel
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                {
                    resultado.Message = "Solo se permiten archivos Excel (.xlsx o .xls)";
                    return BadRequest(resultado);
                }
                // Leer el archivo Excel
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                Console.WriteLine($"2. Stream creado, tamaño: {stream.Length}");

                stream.Position = 0;

                Console.WriteLine($"3. Stream posición reiniciada a: {stream.Position}");

                // Verificar si el stream tiene datos
                if (stream.Length == 0)
                    return BadRequest("Stream vacío");

                // Verificar los primeros bytes para confirmar que es un Excel válido
                byte[] firstBytes = new byte[8];
                stream.Read(firstBytes, 0, 8);
                stream.Position = 0;
                Console.WriteLine($"4. Primeros bytes: {BitConverter.ToString(firstBytes)}");

                using var workbook = new XLWorkbook(stream);
                Console.WriteLine("5. Workbook creado exitosamente");

                var worksheet = workbook.Worksheet(1); // Primera hoja
                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

                if (lastRow < 2)
                {
                    resultado.Message = "El archivo no tiene datos para procesar";
                    return BadRequest(resultado);
                }


                // Leer desde la fila 2 (asumiendo que fila 1 es encabezado)
                for (int row = 2; row <= lastRow; row++)
                {
                    try
                    {
                        var codigo = worksheet.Cell(row, 1).GetString()?.Trim();
                        var nombre = worksheet.Cell(row, 2).GetString()?.Trim();

                        if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(nombre))
                        {
                            resultado.Errores.Add($"Fila {row}: Código o nombre vacío");
                            continue;
                        }



                        var itemDto = new CentroDistribucionDto
                        {
                            Nombre = nombre,
                            Codigo = codigo,
                        };

                        var getProperty = await _service.GetByCodigoAsync(codigo);
                        if (getProperty != null)
                        {
                            resultado.Errores.Add($"Centro ya existe se pasa a la siguiente fila");
                            continue;
                        }

                        CentroDistribucion item = _mapper.Map<CentroDistribucion>(itemDto);

                        itemDto = await _service.CreateAsync(item);

                        resultado.RegistrosValidos++;
                    }
                    catch (Exception ex)
                    {
                        resultado.Errores.Add($"Fila {row}: Error - {ex.Message}");
                    }
                }

                resultado.Message = $"ok, Procesado: {resultado.RegistrosValidos} válidos, {resultado.Errores.Count} errores";
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
