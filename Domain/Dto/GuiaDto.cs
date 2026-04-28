using System.ComponentModel.DataAnnotations.Schema;
using TransporteApi.Models;

namespace Domain.Dto
{
    public class GuiaDto
    {
        public int Id { get; set; } = 0;
        public string Numero_guia { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;
        public int Conductor_id { get; set; } = 0;
        [NotMapped]
        public ConductorDto? Conductor { get; set; }

        [Column("Camion_id")]  // Nombre exacto en la BD
        public int camion_id { get; set; } = 0;
        [NotMapped]
        public CamionDto? Camion { get; set; }
        public int Origen_id { get; set; } = 0;
        [NotMapped]
        public CentroDistribucionDto? Origen { get; set; }
        public int Destino_id { get; set; } = 0;
        [NotMapped]
        public CentroDistribucionDto? Destino { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }



    }
}
