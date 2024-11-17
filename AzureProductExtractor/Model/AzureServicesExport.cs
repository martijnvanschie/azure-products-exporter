using System.Text.Json.Serialization;

public class AzureServicesExport
{
    [JsonPropertyName("exportdate")]
    public DateTime ExportDate { get; set; }
    [JsonPropertyName("exportsource")]
    public string ExportSource { get; set; }
    [JsonPropertyName("categories")]
    public List<Category> Categories { get; set; } = new List<Category>();
}