using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Domain.Dto
{
    public class Rol
    {

        public int Id { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        [JsonIgnore]
        public virtual Collection<RolPermiso> RolesPermisos { get; set; }
    }
}
