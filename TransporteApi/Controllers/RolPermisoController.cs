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
    public class RolPermisoController : ControllerBase
    {

        protected readonly RolPermisoService _service;
        protected readonly IMapper _mapper;

        public RolPermisoController(RolPermisoService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<RolPermisoDto> lista = await _service.GetAllAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            RolPermisoDto item = await _service.GetByIdAsync(id);
            return Ok(item);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RolPermisoDto itemDto)
        {
            RolPermiso item = _mapper.Map<RolPermiso>(itemDto);
            item.Created_at = DateTime.Now;
            itemDto = await _service.CreateAsync(item);
            return Ok(itemDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RolPermisoDto itemDto)
        {
            if (id != itemDto.Id)
                return BadRequest();

            // 1. Obtener la entidad EXISTENTE de la BD
            RolPermiso itemExistente = await _service.FindAsync(id);
            if (itemExistente == null)
                return NotFound();

            // 2. Guardar la fecha original
            var fechaOriginal = itemExistente.Created_at;

            // 3. Mapear el DTO SOBRE la entidad existente
            _mapper.Map(itemDto, itemExistente);

            // 4. Restaurar fecha original y actualizar modificación
            itemExistente.Created_at = fechaOriginal;

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
