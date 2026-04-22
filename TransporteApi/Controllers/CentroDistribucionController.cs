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
    }
}
