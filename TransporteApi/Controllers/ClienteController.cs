using AutoMapper;
using Domain.Dto;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransporteApi.Services;

namespace TransporteApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClienteController : ControllerBase
    {

        protected readonly ClienteService _service;
        protected readonly IMapper _mapper;

        public ClienteController(ClienteService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int empresaId = Convert.ToInt32(User.FindFirst("EmpresaId")?.Value);
            IEnumerable<ClienteDto> lista = await _service.GetAllAsync(empresaId);
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int empresaId = Convert.ToInt32(User.FindFirst("EmpresaId")?.Value);
            ClienteDto item = await _service.GetByIdAsync(id, empresaId);
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ClienteDto itemDto)
        {
            try
            {
                int empresaId = Convert.ToInt32(User.FindFirst("EmpresaId")?.Value);
                Cliente item = _mapper.Map<Cliente>(itemDto);
                item.EmpresaId = empresaId;
                itemDto = await _service.CreateAsync(item);
                return Ok(itemDto);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ClienteDto itemDto)
        {

            Cliente itemExistente = await _service.FindAsync(id);
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

    }
}
