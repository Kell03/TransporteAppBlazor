using AutoMapper;
using Domain.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransporteApi.Models;
using TransporteApi.Services;

namespace TransporteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CamionController : ControllerBase
    {

        protected readonly CamionService _service;
        protected readonly IMapper _mapper;

        public CamionController(CamionService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<CamionDto> lista = await _service.GetAllAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            CamionDto item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CamionDto itemDto)
        {
            try
            {
                Camion item = _mapper.Map<Camion>(itemDto);
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
        public async Task<IActionResult> Put(int id, [FromBody] CamionDto itemDto)
        {
            if (id != itemDto.Id)
                return BadRequest();

            // 1. Obtener la entidad EXISTENTE de la BD
            Camion itemExistente = await _service.FindAsync(id);
            if (itemExistente == null)
                return NotFound();

            // 2. Guardar la fecha original
            var fechaOriginal = itemExistente.Created_at;

            // 3. Mapear el DTO SOBRE la entidad existente
            _mapper.Map(itemDto, itemExistente);

            // 4. Restaurar fecha original y actualizar modificación
            itemExistente.Created_at = fechaOriginal;
            itemExistente.Updated_at = DateTime.Now;

            // 5. Actualizar USANDO la entidad existente
            var resultado = await _service.UpdateAsync(itemExistente);

            return Ok(resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {

            return Ok(await _service.DeleteAsync(id));
        }

    }
}
