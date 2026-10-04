using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class ClienteDto
    {

        public int Id { get; set; }
        public string? RFC { get; set; }
        public string? Razon_social { get; set; }
        public string? Nombre_comercial { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Contacto { get; set; }
        public int? Dias_credito { get; set; }
        public bool? Activo { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }

        public int? EmpresaId { get; set; }
    }
}
