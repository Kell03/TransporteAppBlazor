using AutoMapper;
using BCrypt.Net;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using TransporteApi.Models;
using TransporteApi.Services;

namespace TransporteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuarioController : ControllerBase
    {

        protected readonly UsuarioService _service;
        protected readonly IMapper _mapper;
        private IConfiguration _config;
        public UsuarioController(UsuarioService service, IMapper mapper, IConfiguration config)
        {
            _service = service;
            _mapper = mapper;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int empresaId = Convert.ToInt32(User.FindFirst("EmpresaId")?.Value);
            IEnumerable<UsuarioDto> lista = await _service.GetAllAsync(empresaId);
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int empresaId = Convert.ToInt32(User.FindFirst("EmpresaId")?.Value);
            UsuarioDto item = await _service.GetByIdAsync(id, empresaId);
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsuarioDto itemDto)
        {
            try
            {
                int empresaId = Convert.ToInt32(User.FindFirst("EmpresaId")?.Value);
                Usuario item = _mapper.Map<Usuario>(itemDto);
                item.Password = itemDto.Password;
                item.PasswordHash = BCrypt.Net.BCrypt.HashPassword(itemDto.Password);

                //HashPassword
                // Opcional: No guardar el password en texto plano
                item.Password = null; // Si tienes campo Password

                item.Created_at = DateTime.Now;
                item.EmpresaId = empresaId; // Asignar el EmpresaId del usuario autenticado
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
        [AllowAnonymous]
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

            if (passwordValida)
            {
                loginDto.Id = resultado.Id;
                loginDto.EmpresaId = resultado.EmpresaId;
                loginDto.RolId = resultado.RolId;
                var tokenString = GenerateJSONWebToken(loginDto);
                resultadoDto.Token = tokenString;
            }

            return Ok(resultadoDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {

            return Ok(await _service.DeleteAsync(id));
        }


        private string GenerateJSONWebToken(UsuarioDto userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 🔑 IMPORTANTE: Crear los claims del usuario
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, userInfo.Email),           // Para User.Identity.Name
        new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()), // Para User.FindFirst(ClaimTypes.NameIdentifier)
        new Claim("Id", userInfo.Id.ToString()),              // Claim personalizado
        new Claim("Email", userInfo.Email),                   // Claim personalizado
        new Claim("EmpresaId", userInfo.EmpresaId?.ToString() ?? "") // Si el usuario tiene empresa
    };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,  // 🔑 Aquí van los claims
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
