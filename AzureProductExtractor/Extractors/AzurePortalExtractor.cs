﻿using HtmlAgilityPack;
using Spectre.Console;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace AzureProductExtractor.Extractors
{

    /// <summary>
    /// Tools: http://xpather.com/
    /// </summary>
    internal class AzurePortalExtractor
    {
        private static string XPATH_CATEGORIES = "//li[contains(@class, 'fxs-sidebar-item-category')]";
        private static string XPATH_PROCUCTS = "//li[contains(@class, 'fxs-sidebar-item')]";
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

        public void ExtractFromExport(FileInfo exportFile)
        {
            _export.ExportDate = DateTime.Now;
            _export.ExportSource = "Azure Portal dump";

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(File.ReadAllText(exportFile.FullName));
            var categoryList = htmlDoc.DocumentNode.SelectNodes(XPATH_CATEGORIES);

            foreach (var category in categoryList)
            {
                var name = category.GetAttributeValue("data-category", "not-found");
                AnsiConsole.MarkupLine("[blue]Processing category [[{0}]][/]", name);

                _export.Categories.Add(new Category()
                {
                    Id = NormalizeNameToId(name),
                    Name = NormalizeName(name),
                    Services = new List<AzureService>()
                });
            }

            var productNodes = htmlDoc.DocumentNode.SelectNodes(XPATH_PROCUCTS);

            foreach (HtmlNode productNode in productNodes)
            {
                var categoryName = productNode.GetAttributeValue("data-category", "not-found");
                categoryName = NormalizeName(categoryName);

                var productNameNode = productNode.SelectSingleNode(XPATH_PROCUCTNAME);

                if (productNameNode is not null)
                {
                    AnsiConsole.MarkupLine("[yellow]Processing service [[{0}]][/]", productNameNode.InnerText);
                    var service = new AzureService()
                    {
                        Id = NormalizeNameToId(productNameNode.InnerText),
                        Name = NormalizeName(productNameNode.InnerText)
                    };

                    var cat = _export.Categories.FirstOrDefault(c => c.Name.Equals(categoryName));
                    cat.Services.Add(service);
                }
            }

            string jsonString = JsonSerializer.Serialize(_export, options: new JsonSerializerOptions() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true });
            File.WriteAllText("azureservices.json", jsonString);
        }
    }
}
