using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Dto
{
    public class Usuario
    {

        public int Id { get; set; } = 0;
        public string Nombre { get; set; }
        public string Email { get; set; }
        [NotMapped]
        public string Password { get; set; } 
        public string PasswordHash { get; set; } // ← SOLO HASH
        [NotMapped]
        public string? Token { get; set; } = string.Empty;
        public int RolId { get; set; }

        public Rol Rol { get; set; }

        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
