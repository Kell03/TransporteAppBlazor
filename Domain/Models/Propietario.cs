using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Domain.Dto
{
    public class Propietario
    {
        public int Id { get; set; } 
        public string Nombre { get; set; } = string.Empty;
        public string? NumeroIdentidad { get; set; } 
        public string? Codigo { get; set; } = string.Empty;
        public virtual List<Camion> Camion { get; set; } = [];
        public virtual List<Conductor> Conductor { get; set; } = [];
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
