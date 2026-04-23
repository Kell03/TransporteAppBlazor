using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Dto
{
    public class PropietarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? NumeroIdentidad { get; set; }
        public string? Codigo { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual List<CamionDto> Camion { get; set; } = [];

        [JsonIgnore]
        public virtual List<ConductorDto> Conductor { get; set; } = [];

        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
