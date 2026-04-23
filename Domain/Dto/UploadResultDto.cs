using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class UploadResultDto
    {
        public string Message { get; set; } = string.Empty;
        public int TotalFilas { get; set; } = 0;
        public int RegistrosValidos { get; set; } = 0;
        public List<string> Errores { get; set; } = new List<string>();
    }
}
