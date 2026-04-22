using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TransporteApi.Models;

namespace Domain.Dto
{
    public class CamionDto
    {

        public int Id { get; set; } = 0;
        public string Tipo_Camion { get; set; } = string.Empty;
        public string Placa1 { get; set; } = string.Empty;
        public string Placa2 { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Estado { get; set; } = "Activo";

        public int Propietario_Id { get; set; }
        
        [NotMapped]
        public PropietarioDto? Propietario { get; set; }

        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
