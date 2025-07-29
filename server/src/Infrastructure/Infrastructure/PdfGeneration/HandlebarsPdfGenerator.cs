using System.Globalization;

using Application.Contracts.Services;
using Application.Features.WorkOrders.Query.GetWorkOrderInvoicePdf;

using HandlebarsDotNet;

using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Infrastructure.PdfGeneration;

/// <summary>
/// Handlebars-based PDF generator using PuppeteerSharp for HTML to PDF conversion
/// </summary>
public class HandlebarsPdfGenerator : IInvoicePdfService
{
    private readonly string _templatePath;

    public HandlebarsPdfGenerator()
    {
        // Register custom helpers
        RegisterHelpers();

        // Set template path
        _templatePath = Path.Combine(AppContext.BaseDirectory, "PdfGeneration", "Views", "WorkOrderInvoice.hbs");
    }

    /// <summary>
    /// Generates a PDF invoice for a work order
    /// </summary>
    /// <param name="invoiceData">The work order invoice data to generate the PDF from</param>
    /// <returns>PDF as byte array</returns>
    public async Task<byte[]> GenerateInvoicePdfAsync(GetWorkOrderInvoicePdfDTO invoiceData)
    {
        // Read the template file
        var templateContent = await File.ReadAllTextAsync(_templatePath);

        // Compile the Handlebars template with data
        var htmlContent = CompileTemplate(templateContent, invoiceData);

        // Convert HTML to PDF
        var pdfBytes = await GeneratePdfFromHtmlAsync(htmlContent);

        return pdfBytes;
    }

    /// <summary>
    /// Compiles a Handlebars template with the provided data
    /// </summary>
    /// <param name="templateContent">The Handlebars template content</param>
    /// <param name="data">The data to bind to the template</param>
    /// <returns>Compiled HTML content</returns>
    private static string CompileTemplate(string templateContent, object data)
    {
        var template = Handlebars.Compile(templateContent);
        return template(data);
    }

    /// <summary>
    /// Generates a PDF from HTML content using PuppeteerSharp
    /// </summary>
    /// <param name="htmlContent">The HTML content to convert to PDF</param>
    /// <returns>PDF as byte array</returns>
    private static async Task<byte[]> GeneratePdfFromHtmlAsync(string htmlContent)
    {
        // Download Chromium if not already downloaded
        await new BrowserFetcher().DownloadAsync();

        // Launch browser
        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        // Initialise a page
        using var page = await browser.NewPageAsync();

        await page.SetContentAsync(htmlContent);

        // Wait until dynamic fonts are available in the DOM
        await page.EvaluateExpressionHandleAsync("document.fonts.ready");

        var pdfBytes = await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "10px",
                Right = "10px",
                Bottom = "10px",
                Left = "10px"
            }
        });

        return pdfBytes;
    }

    /// <summary>
    /// Registers custom Handlebars helpers
    /// </summary>
    private static void RegisterHelpers()
    {
        // Equality
        Handlebars.RegisterHelper("eq", (context, arguments) =>
            arguments.Length == 2 && arguments[0]?.ToString() == arguments[1]?.ToString());

        // Format currency
        Handlebars.RegisterHelper("formatCurrency", (context, arguments) =>
            arguments.Length > 0 && decimal.TryParse(arguments[0]?.ToString(), out var value)
                ? value.ToString("C2", CultureInfo.GetCultureInfo("ms-MY"))
                : "RM0.00");

        // Format date
        Handlebars.RegisterHelper("formatDate", (context, arguments) =>
            arguments.Length > 0 && DateTime.TryParse(arguments[0]?.ToString(), out var date)
                ? date.ToString("dd/MM/yyyy")
                : "N/A");
    }
}