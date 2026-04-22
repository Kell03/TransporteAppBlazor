using AutoMapper;
using Domain.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



using TransporteApi.Models;
using TransporteApi.Services;

namespace TransporteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {

        protected readonly RolService _service;
        protected readonly IMapper _mapper;

        public RolController(RolService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<RolDto> lista = await _service.GetAllAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            RolDto item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RolDto itemDto)
        {
            try
            {
                Rol item = _mapper.Map<Rol>(itemDto);
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
        public async Task<IActionResult> Put(int id, [FromBody] RolDto itemDto)
        {
            if (id != itemDto.Id)
                return BadRequest();

            // 1. Obtener la entidad EXISTENTE de la BD
            Rol itemExistente = await _service.FindAsync(id);
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
