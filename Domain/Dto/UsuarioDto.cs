using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TransporteApi.Models;

namespace Domain.Dto
{
    public class UsuarioDto
    {

        public int Id { get; set; } = 0;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [NotMapped]
        public string Password { get; set; } = string.Empty;
        public int RolId { get; set; } = 0;

        [NotMapped]
        public RolDto? Rol { get; set; }

        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
