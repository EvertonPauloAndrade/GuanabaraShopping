using System.Text.Json.Serialization;

namespace ClienteManager.Models
{
    public class Cidade
    {
        [JsonPropertyName("ID")]
        public string ID { get; set; } = string.Empty;
        
        [JsonPropertyName("Nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("Estado")]
        public string Estado { get; set; } = string.Empty;
    }
}