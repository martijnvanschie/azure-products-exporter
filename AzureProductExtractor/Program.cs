using HtmlAgilityPack;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

// Using the following guide.
// https://www.scrapingbee.com/blog/web-scraping-csharp/
// https://www.scrapingbee.com/blog/practical-xpath-for-web-scraping/

List<Category> categories = new List<Category>();

var fullUrl = "https://azure.microsoft.com/en-us/products/";
HttpClient client = new HttpClient();
var response = await client.GetStringAsync(fullUrl);

HtmlDocument htmlDoc = new HtmlDocument();
htmlDoc.LoadHtml(response);

var productList = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@id, 'products-list')]");

var currentCategory = "";
foreach (HtmlNode item in productList.ChildNodes)
{
    if (item.Name == "div" && item.HasClass("row") && item.HasClass("column"))
    {
        var category = item.SelectSingleNode(".//h2[contains(@class, 'product-category')]");
        Console.WriteLine(category.InnerText);

        if (categories.Any(c => c.Name.Equals(category.InnerText)) == false)
        {
            currentCategory= category.InnerText;
            categories.Add(new Category()
            {
                Id = category.InnerText.ToLower().Replace(" ", "-").Replace("+", ""),
                Name = category.InnerText,
                Products = new List<Product>()
            });

            continue;
        }
    }

    if (item.Name == "div" && item.HasClass("row") && item.HasClass("row-divided"))
    {
        var products = item.SelectNodes(".//span");
        foreach (var product in products)
        {
            AddProductToCategory(currentCategory, product.InnerText);
        }
    }

    if (item.Name == "div" && item.HasClass("row") && item.HasClass("row-size2"))
    {
        var products = item.SelectNodes(".//span");
        foreach (var product in products)
        {
            AddProductToCategory(currentCategory, product.InnerText);
        }
    }
}

string jsonString = JsonSerializer.Serialize(categories, options: new JsonSerializerOptions() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true } );

File.WriteAllText("azureproducts.json", jsonString);

return;

void AddProductToCategory(string categoryName, string productName)
{
    if (string.IsNullOrEmpty(categoryName) || string.IsNullOrEmpty(productName))
    {
        return;
    }

    if (categories.Any(c => c.Name.Equals(categoryName)))
    {
        var category = categories.FirstOrDefault(c => c.Name.Equals(categoryName));

        if (category is not null && !category.Products.Any(p => p.Name.Equals(productName)))
        {
            category.Products.Add(new Product()
            {
                Id = productName.ToLower().Replace(" ", "-").Replace("+", ""),
                Name = productName
            });
        }
    }
}

public class Category
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("skills")]
    public List<Product> Products { get; set;}
}

public class Product
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}