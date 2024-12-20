using HtmlAgilityPack;
using Spectre.Console;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace AzureProductExtractor.Extractors
{
    internal class AzureUpdatesExtractor
    {
        // //li/div[@class='product-filters-parent']/div[@class='product-filters-header']/div[@class='ocr-input ocr-input--type-checkbox d-flex product_parent_option']/label[@class='ocr-input__label azure_dopdown_parent_label']/text()

        private static string XPATH_CATEGORIES = ".//div[@class='product-filters-parent']";
        private static string XPATH_CATEGORy_NAME = "./div[@class='product-filters-header']/div[@class='ocr-input ocr-input--type-checkbox d-flex product_parent_option']/label[@class='ocr-input__label azure_dopdown_parent_label']/text()";

        private static string XPATH_PROCUCTS = "./div[@class='product-filters-children']/div[@class='ocr-input ocr-input--type-checkbox d-flex align-items-start']/label[@class='ocr-input__label azure_dopdown_label'][1]/text()";
        private static string XPATH_PROCUCTNAME = ".//div[contains(@class, 'fxs-sidebar-label-name')]";

        AzureServicesExport _export = new AzureServicesExport();

        private string NormalizeName(string name)
        {
            var normalizeName = name.Trim();
            normalizeName = normalizeName.Replace("&amp;", "and");
            normalizeName = normalizeName.Replace("™", "");
            normalizeName = normalizeName.Replace("®", "");

            return normalizeName;
        }

        private string NormalizeNameToId(string name)
        {
            var normalizeName = name.ToLower();
            normalizeName = normalizeName.Trim();
            normalizeName = normalizeName.Replace("&amp;", "and");
            normalizeName = normalizeName.Replace("™", "");
            normalizeName = normalizeName.Replace("®", "");
            normalizeName = normalizeName.Replace(" - ", "-");
            normalizeName = normalizeName.Replace(" + ", "-");
            normalizeName = normalizeName.Replace(" ", "-");

            return normalizeName;
        }

        public async Task ExtractFromExport(FileInfo exportFile)
        {
            _export.ExportDate = DateTime.Now;
            _export.ExportSource = "Azure Portal dump";

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(File.ReadAllText(exportFile.FullName));

            var categoryList = htmlDoc.DocumentNode.SelectNodes(XPATH_CATEGORIES);

            foreach (var categoryNode in categoryList)
            {
                var name = categoryNode.SelectSingleNode(XPATH_CATEGORy_NAME).InnerText;
                AnsiConsole.MarkupLine("[blue]Processing category [[{0}]][/]", name);

                var cat = new Category()
                {
                    Id = NormalizeNameToId(name),
                    Name = NormalizeName(name),
                    Services = new List<AzureService>()
                };

                var productNodes = categoryNode.SelectNodes(XPATH_PROCUCTS);

                foreach (HtmlNode productNode in productNodes)
                {
                    var categoryName = productNode.InnerText;
                    categoryName = NormalizeName(categoryName);

                    AnsiConsole.MarkupLine("[yellow]Processing service [[{0}]][/]", categoryName);
                    var service = new AzureService()
                    {
                        Id = NormalizeNameToId(categoryName),
                        Name = NormalizeName(categoryName)
                    };

                    cat.Services.Add(service);
                }

                _export.Categories.Add(cat);
            }

            string jsonString = JsonSerializer.Serialize(_export, options: new JsonSerializerOptions() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true });
            File.WriteAllText("azureservices.json", jsonString);
        }
    }
}
