using AzureProductExtractor.Extractors;
using Spectre.Console;

// Using the following guide.
// https://www.scrapingbee.com/blog/web-scraping-csharp/
// https://www.scrapingbee.com/blog/practical-xpath-for-web-scraping/

AnsiConsole.Write(
    new FigletText("Azure Services Importer v1.0")
    .Centered()
    .Color(Color.Yellow));

AzurePortalExtractor extractor = new AzurePortalExtractor();
extractor.ExtractFromExport(new FileInfo("azure-portal-export.html"));

Environment.Exit(0);