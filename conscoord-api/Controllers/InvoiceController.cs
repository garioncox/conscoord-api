using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<List<InvoiceInfoDTO>>> GetInvoiceInfoByCompanyTimePeriod(int companyId, IInvoiceService interfaceService)
    {
        DateTime date2020 = new DateTime(2020, 7, 15, 10, 30, 0, DateTimeKind.Utc);
        DateTime date2026 = new DateTime(2026, 3, 22, 17, 45, 0, DateTimeKind.Utc);

        var result = await interfaceService.GetInvoiceInfoByCompanyTimePeriod(companyId, date2020, date2026);
        return Ok(result);
    }

    [HttpPost]
    public void GeneratePDF()
    {
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);

        // Define fonts
        XFont titleFont = new XFont("Verdana", 18, XFontStyleEx.Bold);
        XFont headerFont = new XFont("Verdana", 14, XFontStyleEx.Bold);
        XFont normalFont = new XFont("Verdana", 12);
        XFont totalFont = new XFont("Verdana", 16, XFontStyleEx.Bold);

        // Title
        gfx.DrawString("Invoice", titleFont, XBrushes.Black,
            new XRect(0, 40, page.Width, page.Height),
            XStringFormats.TopCenter);

        // Company Name
        gfx.DrawString("Acme Corporation", titleFont, XBrushes.Black,
            new XRect(0, 80, page.Width, page.Height),
            XStringFormats.TopCenter);

        // Invoice Details (Project 1)
        double yPosition = 120; // Start position for first project
        gfx.DrawString("Project 1: Web Development", headerFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString("Days worked: 5", normalFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString("Rate: $200/day", normalFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString("Total: $1000", normalFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);

        // Horizontal Line After Project 1
        yPosition += 30; // Add space before the line
        gfx.DrawLine(XPens.Black, 40, yPosition, page.Width - 40, yPosition);

        // Project 2
        yPosition += 10;
        gfx.DrawString("Project 2: Mobile App Development", headerFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString("Days worked: 7", normalFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString("Rate: $250/day", normalFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString("Total: $1750", normalFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);

        // Horizontal Line After Project 2
        yPosition += 30;
        gfx.DrawLine(XPens.Black, 40, yPosition, page.Width - 40, yPosition);

        // Project 3
        yPosition += 10;
        gfx.DrawString("Project 3: Database Optimization", headerFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString("Days worked: 3", normalFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString("Rate: $300/day", normalFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString("Total: $900", normalFont, XBrushes.Black,
            new XRect(40, yPosition, page.Width, page.Height),
            XStringFormats.TopLeft);

        // Horizontal Line After Project 3
        yPosition += 30;
        gfx.DrawLine(XPens.Black, 40, yPosition, page.Width - 40, yPosition);

        // Grand Total (Bottom Right)
        double grandTotal = 1000 + 1750 + 900;
        yPosition += 40;
        gfx.DrawString("Grand Total: $" + grandTotal, totalFont, XBrushes.Black,
            new XRect(page.Width - 200, yPosition, 200, 40),
            XStringFormats.TopRight);

        // Save the document
        string filename = "Invoice.pdf";
        document.Save(filename);

        Console.WriteLine($"Invoice generated and saved as {filename}");

    }
}
