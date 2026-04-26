using AutoMapper;
using ClosedXML.Excel;
using Domain.Dto;
using Domain.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransporteApi.Models;
using TransporteApi.Services;

namespace TransporteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuiasController : ControllerBase
    {

        protected readonly GuiaService _service;
        protected readonly IMapper _mapper;

        public GuiasController(GuiaService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<GuiaDto> lista = await _service.GetAllAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            GuiaDto item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GuiaDto itemDto)
        {
            try
            {
                Guia item = _mapper.Map<Guia>(itemDto);

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


        [HttpPost("export/excel")]
        public async Task<IActionResult> ExportarGuiasExcel([FromBody] ExportRequest exportRequest)
        {
           

            // Cargar guías con sus relaciones
            var guiasQuery = await _service.GetAllAsync();

            // Aplicar QuickFilter si existe
            if (!string.IsNullOrWhiteSpace(exportRequest?.SearchString))
            {
                var searchString = exportRequest.SearchString;
                guiasQuery = guiasQuery.Where(x =>
                    x.Numero_guia.Contains(searchString) ||
                    x.Conductor.NombreCompleto.Contains(searchString) ||
                    x.Origen.Nombre.Contains(searchString) ||
                    x.Destino.Nombre.Contains(searchString));
            }

            // Aplicar filtros específicos desde FilterDefinitions
            if (exportRequest?.Filtros != null && exportRequest.Filtros.Any())
            {
                foreach (var filtro in exportRequest.Filtros)
                {
                    if (string.IsNullOrWhiteSpace(filtro.Value?.ToString()))
                        continue;

                    guiasQuery = AplicarFiltro(guiasQuery.AsQueryable(), filtro);
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
                    worksheet.Cell(row, 5).Value = guia.Origen?.Nombre ?? "Sin definir";
                    worksheet.Cell(row, 6).Value = guia.Destino?.Nombre ?? "Sin definir";
                    worksheet.Cell(row, 7).Value = guia.Fecha.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(row, 8).Value = guia.Tipo;
                    worksheet.Cell(row, 9).Value = guia.Status;
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


        private IQueryable<GuiaDto> AplicarFiltro(IQueryable<GuiaDto> query, FilterDefinitionDto filtro)
        {
            var propertyName = filtro.PropertyName;
            var operatorType = filtro.Operator;
            var value = filtro.Value?.ToString();

            if (string.IsNullOrWhiteSpace(value))
                return query;

            switch (propertyName)
            {
                case "Numero_guia":
                    return query.Where(x => x.Numero_guia.Contains(value));

                case "Tipo":
                    return query.Where(x => x.Tipo == value);

                case "Status":
                    return query.Where(x => x.Status == value);

                case "Origen.Nombre":
                    return query.Where(x => x.Origen.Nombre.Contains(value));

                case "Destino.Nombre":
                    return query.Where(x => x.Destino.Nombre.Contains(value));

                case "Conductor.NombreCompleto":
                    return query.Where(x => x.Conductor.NombreCompleto.Contains(value));

                case nameof(GuiaDto.Fecha):
                    if (DateTime.TryParse(value, out var fecha))
                    {
                       
                        if (operatorType == "is not")
                            return query.Where(x => x.Fecha.Date != fecha.Date);
                        if (operatorType == "is")
                            return query.Where(x => x.Fecha.Date == fecha.Date);
                    }
                    return query; // Si no se pudo parsear la fecha

                default:
                    return query;
            }
        }
     }



}
