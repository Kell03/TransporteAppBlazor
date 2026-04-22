using System.Collections.ObjectModel;
using TransporteApi.Models;

namespace Domain.Dto
{
    public class RolDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public virtual Collection<RolPermisoDto> RolesPermisos { get; set; } = new Collection<RolPermisoDto>();
    }
}
