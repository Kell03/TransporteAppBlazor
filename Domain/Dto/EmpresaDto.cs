using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class EmpresaDto
    {
        public int Id { get; set; } = 0;
        public string Nombre { get; set; }

        [JsonIgnore]
        public virtual List<UsuarioDto> Usuarios { get; set; } = [];
    }
}
