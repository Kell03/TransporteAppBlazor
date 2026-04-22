using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.Dto
{
    public class Guia
    {

        public int Id { get; set; } = 0;
        public string Numero_guia { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        [Column("Conductor_id")]  // Nombre exacto en la BD
        public int Conductor_id { get; set; } = 0;
        public Conductor Conductor { get; set; }

        [Column("Camion_id")]  // Nombre exacto en la BD
        public int Camion_id { get; set; } = 0;
        public Camion Camion { get; set; }

        [Column("Origen_id")]  // Nombre exacto en la BD
        public int Origen_id { get; set; } = 0;
        public CentroDistribucion Origen { get; set; }

        [Column("Destino_id")]  // Nombre exacto en la BD
        public int Destino_id { get; set; } = 0;
        public CentroDistribucion Destino { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }

}
