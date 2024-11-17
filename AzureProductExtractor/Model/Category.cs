using System.Text.Json.Serialization;

public class Category
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("services")]
    public List<AzureService> Services { get; set; } = new List<AzureService>();
}
