using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class GuiaTelegramDto
    {
        [JsonPropertyName("Numero_guia")]
        public string Numero_guia { get; set; } = string.Empty;

        [JsonPropertyName("Tipo")]
        public string Tipo { get; set; } = string.Empty;

        [JsonPropertyName("Descripcion")]
        public string? Descripcion { get; set; }

        [JsonPropertyName("ClienteId")]
        public int ClienteId { get; set; }

        [JsonPropertyName("NombreOrigen")]
        public string NombreOrigen { get; set; } = string.Empty;

        [JsonPropertyName("NombreDestino")]
        public string NombreDestino { get; set; } = string.Empty;

        [JsonPropertyName("CedulaConductor")]
        public string CedulaConductor { get; set; } = string.Empty;

        [JsonPropertyName("PlacaCamion")]
        public string PlacaCamion { get; set; } = string.Empty;

        [JsonPropertyName("EmpresaId")]
        public int EmpresaId { get; set; }
    }
}
