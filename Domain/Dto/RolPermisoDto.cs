using System.Text.Json.Serialization;
using TransporteApi.Models;

namespace Domain.Dto
{
    public class RolPermisoDto
    {

        public int Id { get; set; }

        public int Rol_Id { get; set; }
        public string Permiso { get; set; }

        public DateTime Created_at { get; set; } = DateTime.Now;

        [JsonIgnore]
        public virtual RolDto? Rol { get; set; }
    }
}
