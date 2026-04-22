using System.Text.Json.Serialization;
using TransporteApi.Models;

namespace Domain.Dto
{
    public class ConductorDto
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cedula { get; set; }
        public int Propietario_Id { get; set; }

        public PropietarioDto? Propietario { get; set; }
        public DateTime Fecha_alta { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

    }
}
