using AutoMapper;
using Domain.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Dto;
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
    }



}
