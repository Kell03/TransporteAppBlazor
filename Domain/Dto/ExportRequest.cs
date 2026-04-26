using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class ExportRequest
    {

        public List<FilterDefinitionDto> Filtros { get; set; }
        public string? SearchString { get; set; } = string.Empty;
        public string Formato { get; set; }
    }
}
