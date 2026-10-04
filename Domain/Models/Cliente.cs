using Domain.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Cliente
    {

        public int Id { get; set; }
        public string? RFC { get; set; }
        [Column("razon_social")]  // Nombre exacto en la BD

        public string? Razon_social { get; set; }
        [Column("nombre_comercial")]  // Nombre exacto en la BD

        public string? Nombre_comercial  { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Contacto { get; set; }
        [Column("dias_credito")]  // Nombre exacto en la BD

        public int? Dias_credito { get; set; }
        public bool? Activo { get; set; }

        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        public int? EmpresaId { get; set; }

        public ICollection<CentroDistribucion> Centros { get; set; } = new List<CentroDistribucion>();
        public ICollection<Guia> Guias { get; set; } = new List<Guia>();
    }
}
