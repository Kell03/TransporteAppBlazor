using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Dto
{
    public class CentroDistribucionDto
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool Status { get; set; }

        public int? EmpresaId { get; set; }
        public int? Kilometraje { get; set; }

        [Column("Cliente_id")]  // Nombre exacto en la BD
        public int cliente_id { get; set; } = 0;
        [NotMapped]
        public ClienteDto? Cliente { get; set; }
    }
}
