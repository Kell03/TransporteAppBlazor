using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Dto
{
    public class Camion
    {

        public int Id { get; set; } = 0;
        public string Tipo_Camion { get; set; }
        public string Placa1 { get; set; }
        public string Placa2 { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Color { get; set; }
        public string Estado { get; set; }
        public int Propietario_Id { get; set; }

        [JsonIgnore]
        public Propietario Propietario { get; set; }

        [JsonIgnore]
        public Conductor? Conductor { get; set; }

        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}

