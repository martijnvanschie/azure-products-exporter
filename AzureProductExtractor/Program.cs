using AzureProductExtractor.Extractors;
using Spectre.Console;

// Using the following guide.
// https://www.scrapingbee.com/blog/web-scraping-csharp/
// https://www.scrapingbee.com/blog/practical-xpath-for-web-scraping/

AnsiConsole.Write(
    new FigletText("Azure Services Importer v1.0")
    .Centered()
    .Color(Color.Yellow));

AzureUpdatesExtractor extractor = new AzureUpdatesExtractor();
await extractor.ExtractFromExport(new FileInfo("azure-updates-export.html"));

Environment.Exit(0);