using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


using TransporteApi.Models;
using TransporteApi.Services;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;
using System.Reflection.Metadata;
using Domain.Dto;

namespace TransporteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        protected readonly UsuarioService _service;
        protected readonly IMapper _mapper;

        public UsuarioController(UsuarioService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<UsuarioDto> lista = await _service.GetAllAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            UsuarioDto item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsuarioDto itemDto)
        {
            try
            {
                Usuario item = _mapper.Map<Usuario>(itemDto);
                item.Password = itemDto.Password;
                item.PasswordHash = BCrypt.Net.BCrypt.HashPassword(itemDto.Password);

                //HashPassword
                // Opcional: No guardar el password en texto plano
                item.Password = null; // Si tienes campo Password

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
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioDto itemDto)
        {
            
            // 1. Obtener la entidad EXISTENTE de la BD
            Usuario itemExistente = await _service.FindAsync(id);
            if (itemExistente == null)
                return NotFound();

            // 2. Guardar la fecha original
            var fechaOriginal = itemExistente.Created_at;

            // 3. Mapear el DTO SOBRE la entidad existente
            _mapper.Map(itemDto, itemExistente);

            bool esValida = BCrypt.Net.BCrypt.Verify(itemDto.Password, itemExistente.PasswordHash);
            if (esValida)
            {
                itemExistente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(itemDto.Password);

                //HashPassword
                // Opcional: No guardar el password en texto plano
                itemExistente.Password = null; // Si tienes campo Password

            }


            // 4. Restaurar fecha original y actualizar modificación
            itemExistente.Created_at = fechaOriginal;
            itemExistente.Updated_at = DateTime.Now;

            // 5. Actualizar USANDO la entidad existente
            var resultado = await _service.UpdateAsync(itemExistente);

            return Ok(resultado);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

   
            var resultado = await _service.Auth(loginDto.Email);

            if (resultado == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            bool passwordValida = BCrypt.Net.BCrypt.Verify(loginDto.Password, resultado.PasswordHash);

            if (passwordValida == false)
                return Unauthorized(new { message = "Credenciales inválidas" });

           

            var resultadoDto = _mapper.Map<UsuarioDto>(resultado);

            return Ok(resultadoDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {

            return Ok(await _service.DeleteAsync(id));
        }
    }
}
