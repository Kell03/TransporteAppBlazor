using AutoMapper;
using ClosedXML.Excel;
using Domain.Dto;
using Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TransporteApi.Models;
using TransporteApi.Services;

namespace TransporteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GuiasController : ControllerBase
    {

        protected readonly GuiaService _service;
        protected readonly PropietarioService _propietarioService;
        protected readonly CamionService _camionService;
        protected readonly ConductorService _conductorservice;
        protected readonly CentroDistribucionService _Cdservice;

        protected readonly IMapper _mapper;

        public GuiasController(GuiaService service, CamionService camionService,
            ConductorService conductor, PropietarioService propietarioService, CentroDistribucionService cdService, IMapper mapper)
        {
            _service = service;
            _camionService = camionService;
            _conductorservice = conductor;
            _propietarioService = propietarioService;
            _Cdservice = cdService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int empresaId = Convert.ToInt32(User.FindFirst("EmpresaId")?.Value);
            IEnumerable<GuiaDto> lista = await _service.GetAllAsync(empresaId);
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int empresaId = Convert.ToInt32(User.FindFirst("EmpresaId")?.Value);
            GuiaDto item = await _service.GetByIdAsync(id, empresaId);
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GuiaDto itemDto)
        {
            try
            {
                int empresaId = Convert.ToInt32(User.FindFirst("EmpresaId")?.Value);
                Guia item = _mapper.Map<Guia>(itemDto);
                item.EmpresaId = empresaId;
                item.Created_at = DateTime.Now;
                itemDto = await _service.CreateAsync(item);
                return Ok(itemDto);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] GuiaDto itemDto)
        {

            Guia itemExistente = await _service.FindAsync(id);
            if (itemExistente == null)
                return NotFound();

            var fechaOriginal = itemExistente.Created_at;
            _mapper.Map(itemDto, itemExistente);

            itemExistente.Created_at = fechaOriginal;
            itemExistente.Updated_at = DateTime.Now;

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

                int empresaId = Convert.ToInt32(User.FindFirst("EmpresaId")?.Value);
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
                        var conductor = worksheet.Cell(row, 1).GetString()?.Trim();
                        if (string.IsNullOrEmpty(conductor))
                        {
                            resultado.Errores.Add($"Fila {row}: Cedula Vacia");
                            continue;
                        }

                        var placa = worksheet.Cell(row, 2).GetString()?.Trim();
                        if (string.IsNullOrEmpty(placa))
                        {
                            resultado.Errores.Add($"Fila {row}: Placa Vacia");
                            continue;
                        }

                       
                        var codigoPropietario = worksheet.Cell(row, 4).GetString()?.Trim();
                        if (string.IsNullOrEmpty(codigoPropietario))
                        {
                            resultado.Errores.Add($"Fila {row}: codigo propietario Vacio");
                            continue;
                        }


                        var nroGuia = worksheet.Cell(row, 5).GetString()?.Trim();
                        if (string.IsNullOrEmpty(nroGuia))
                        {
                            resultado.Errores.Add($"Fila {row}: numero Guia Vacio");
                            continue;
                        }


                        var origen = worksheet.Cell(row, 6).GetString()?.Trim();
                        if (string.IsNullOrEmpty(origen))
                        {
                            resultado.Errores.Add($"Fila {row}: origen Vacio");
                            continue;
                        }


                        var destino = worksheet.Cell(row, 7).GetString()?.Trim();
                        if (string.IsNullOrEmpty(destino))
                        {
                            resultado.Errores.Add($"Fila {row}: destino Vacio");
                            continue;
                        }


                        DateTime fecha = worksheet.Cell(row, 8).GetValue<DateTime>();
                        


                        var condicion = worksheet.Cell(row, 9).GetString()?.Trim();
                        if (string.IsNullOrEmpty(condicion))
                        {
                            resultado.Errores.Add($"Fila {row}: condicion Vacia");
                            continue;
                        }

                        var status = worksheet.Cell(row, 10).GetString()?.Trim();
                        if (string.IsNullOrEmpty(status))
                        {
                            resultado.Errores.Add($"Fila {row}: status Vacio");
                            continue;
                        }


                        var guia = await _service.GetByNumeroAsync(nroGuia, empresaId);

                        if (guia != null)
                        {
                           // resultado.Errores.Add($"Guia ya existente se pasa a la siguiente fila");
                            continue;
                        }

                        var propietario = await _propietarioService.GetByCodigoAsync(codigoPropietario, empresaId);

                        if (propietario == null)
                        {
                            resultado.Errores.Add($"Fila {row}: Propietario no encontrado ");
                            continue;
                        }

                        var camion = await _camionService.GetByPlacaAsync(placa, empresaId);
                        if (camion == null)
                        {
                            resultado.Errores.Add($"Fila {row}: Camion no encontrado ");
                            continue;
                        }

                        var chofer = await _conductorservice.GetByNombreAsync(conductor, empresaId);
                        if (chofer == null)
                        {
                            resultado.Errores.Add($"Fila {row}: Conductor no encontrado ");
                            continue;
                        }

                        var getcentro = await _Cdservice.GetByCodigoAsync(origen);
                        if (getcentro == null)
                        {
                            resultado.Errores.Add($"Fila {row}: Centro origen no encontrado s");
                            continue;
                        }

                        var getcentrodestino = await _Cdservice.GetByCodigoAsync(destino);
                        if (getcentrodestino == null)
                        {
                            resultado.Errores.Add($"Fila {row}:Centro destino no encontrado ");
                            continue;
                        }

                        var itemDto = new GuiaDto
                        {
                            Numero_guia = nroGuia,
                            Tipo = condicion,
                            Status = status,
                            Conductor_id = chofer.Id,
                            Origen_id = getcentro.Id,
                            Destino_id = getcentrodestino.Id,
                            camion_id = camion.Id,
                            Fecha = fecha,
                            Created_at = DateTime.Now,
                            EmpresaId = empresaId
                        };



                        Guia item = _mapper.Map<Guia>(itemDto);
                        itemDto = await _service.CreateAsync(item);

                        resultado.RegistrosValidos++;
                    }
                    catch (Exception ex)
                    {

                        bool esDuplicado = ex.Message.Contains("Duplicate") ||
                                           (ex.InnerException != null && ex.InnerException.Message.Contains("Duplicate"));

                        if (!esDuplicado)
                        {
                            resultado.Errores.Add($"Fila {row}: Error - {ex.Message}");
                        }
                        
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



        [HttpPost("export/excel")]
        public async Task<IActionResult> ExportarGuiasExcel([FromBody] ExportRequest exportRequest)
        {

            int empresaId = Convert.ToInt32(User.FindFirst("EmpresaId")?.Value);
            // Cargar guías con sus relaciones
            var guiasQuery = await _service.GetAllAsync(empresaId);

            // Aplicar QuickFilter si existe
            if (!string.IsNullOrWhiteSpace(exportRequest?.SearchString))
            {
                var searchString = exportRequest.SearchString.ToLower(); // ← Convertido UNA vez

                guiasQuery = guiasQuery.Where(x =>
                    x.Numero_guia.ToLower().Contains(searchString) ||           // ← Campo convertido
                    x.Conductor.Nombre.ToLower().Contains(searchString) ||      // ← Campo convertido
                    x.Conductor.Apellido.ToLower().Contains(searchString) ||    // ← Campo convertido
                    x.Conductor.NombreCompleto.ToLower().Contains(searchString) ||
                    x.Tipo.ToLower().Contains(searchString) ||
                    x.Status.ToLower().Contains(searchString) ||
                    (x.Descripcion != null && x.Descripcion.ToLower().Contains(searchString)) ||
                    x.Origen.Nombre.ToLower().Contains(searchString) ||
                    x.Destino.Nombre.ToLower().Contains(searchString));
            }

            // Aplicar filtros específicos desde FilterDefinitions
            if (exportRequest?.Filtros != null && exportRequest.Filtros.Any())
            {
                foreach (var filtro in exportRequest.Filtros)
                {
                    if (string.IsNullOrWhiteSpace(filtro.Value?.ToString()))
                        continue;

                    guiasQuery =  _service.AplicarFiltro(guiasQuery.AsQueryable(), filtro);
                }
            }

            // Ejecutar consulta con todos los filtros aplicados
            var listaGuias =  guiasQuery.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Guias");

                // Definir encabezados
                var headers = new[]
                {
                "Chofer",
                "Placa",
                "Propietario",
                "N° Guía",
                "Origen",
                "Destino",
                "Fecha",
                "Condicion",
                "Status",
            };

                if(empresaId == 2)
                {
                    headers = headers.Append("Recorrido KM Ida").ToArray();
                    headers = headers.Append("Recorrido KM Vuelta").ToArray();
                }

                // Estilo de encabezados
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(1, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    cell.Style.Font.FontColor = XLColor.Black;
                }

                // Llenar datos
                int row = 2;
                foreach (var guia in listaGuias)
                {

                    worksheet.Cell(row, 1).Value = guia.Conductor?.NombreCompleto ?? "Sin asignar";
                    worksheet.Cell(row, 2).Value = guia.Camion?.Placa1 ?? "Sin asignar";
                    worksheet.Cell(row, 3).Value = guia.Camion?.Propietario?.Codigo ?? "Sin asignar";
                    worksheet.Cell(row, 4).Value = guia.Numero_guia;
                    if (empresaId == 2)
                    {

                        worksheet.Cell(row, 5).Value = $"{guia.Origen?.Codigo}-{guia.Origen?.Nombre}" ?? "Sin definir";
                        worksheet.Cell(row, 6).Value = $"{guia.Destino?.Codigo}-{guia.Destino?.Nombre}" ?? "Sin definir";
                    }
                    else
                    {
                        worksheet.Cell(row, 5).Value = guia.Origen?.Codigo ?? "Sin definir";
                        worksheet.Cell(row, 6).Value = guia.Destino?.Codigo ?? "Sin definir";

                    }
                    worksheet.Cell(row, 7).Value = guia.Fecha.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 8).Value = guia.Tipo;
                    worksheet.Cell(row, 9).Value = guia.Status;
                    if (empresaId == 2)
                    {
                        worksheet.Cell(row, 10).Value = guia.Destino?.Kilometraje;
                        worksheet.Cell(row, 11).Value = guia.Destino?.Kilometraje;

                    }
                    row++;
                }

                // Ajustar columnas
                worksheet.Columns().AdjustToContents();

                // Aplicar bordes a toda la tabla
                var rango = worksheet.Range(1, 1, row - 1, headers.Length);
                rango.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Congelar la primera fila (encabezados)
                worksheet.SheetView.FreezeRows(1);

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"Guias_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(
                    stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
        }


     
     }



}
