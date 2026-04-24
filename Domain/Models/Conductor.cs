using System.Text.Json.Serialization;

namespace Domain.Dto
{
    public class Conductor
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cedula { get; set; }
        public int Propietario_Id { get; set; }

        public int? Camion_Id { get; set; } = 0;

        [JsonIgnore]
        public Camion? Camion { get; set; } 

        [JsonIgnore]
        public Propietario? Propietario { get; set; }
        public DateTime Fecha_alta { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
