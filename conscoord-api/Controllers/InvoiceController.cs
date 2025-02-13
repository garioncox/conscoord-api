using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using conscoord_api.Utils;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly RoleUtils _RoleUtils;

    public InvoiceController(IInvoiceService invoiceService, RoleUtils roleUtils)
    {
        _invoiceService = invoiceService;
        _RoleUtils = roleUtils;
    }

    [HttpPost("getInvoicePreview")]
    public async Task<ActionResult<List<InvoiceInfoDTO>>> GetInvoiceInfoByCompanyTimePeriod(InvoiceDTO DTO)
    {
        var user = HttpContext.User;
        var hasPerms = await _RoleUtils.HasPerms(user, [Role.ADMIN_ROLE]);
        if (!hasPerms) { return NotFound(); }

        var startDateValid = DateTime.TryParseExact(DTO.startDate, ["yyyy/MM/dd", "yyyy/MM/d"], null, System.Globalization.DateTimeStyles.None, out var startDate);
        var endDateValid = DateTime.TryParseExact(DTO.endDate, ["yyyy/MM/dd", "yyyy/MM/d"], null, System.Globalization.DateTimeStyles.None, out var endDate);

        if (!startDateValid || !endDateValid)
        {
            return BadRequest("Please make sure that the date format is YYYY/MM/DD format");
        }

        var result = await _invoiceService.GetInvoiceInfoByCompanyTimePeriod(DTO);
        return Ok(result);
    }

    [HttpPost("generateInvoice")]
    public async Task<IActionResult> GeneratePDF(IInvoiceService interfaceService, InvoiceDTO DTO)
    {
        var user = HttpContext.User;
        var hasPerms = await _RoleUtils.HasPerms(user, [Role.ADMIN_ROLE]);
        if (!hasPerms) { return NotFound(); }

        var startDateValid = DateTime.TryParseExact(
            DTO.startDate,
            new string[] { "yyyy/MM/dd", "yyyy/MM/dd" },
            null,
            System.Globalization.DateTimeStyles.None,
            out var startDate
        );

        var endDateValid = DateTime.TryParseExact(
            DTO.endDate,
            new string[] { "yyyy/MM/dd", "yyyy/MM/dd" },
            null,
            System.Globalization.DateTimeStyles.None,
            out var endDate
        );

        if (!startDateValid || !endDateValid)
        {
            return BadRequest("Please make sure that the date format is YYYY/MM/DD format");
        }

        double hoursCounter = 0;
        double grandTotal = 0;
        var maxYPosition = 750;
        var invoicedata = await interfaceService.GetInvoiceInfoByCompanyTimePeriod(DTO);

        foreach (var data in invoicedata)
        {
            foreach (var shift in data.shiftsByProject)
            {
                foreach (var employee in shift.employeesByShift)
                {
                    grandTotal += employee.hoursWorked * 75;
                }
            }
        }


        PdfDocument document = new PdfDocument();
        var page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);

        // Define fonts
        XFont titleFont = new XFont("Verdana", 18, XFontStyleEx.Bold);
        XFont headerFont = new XFont("Arial", 14, XFontStyleEx.Bold);
        XFont subHeaderFont = new XFont("Arial", 12, XFontStyleEx.Bold);
        XFont normalFont = new XFont("Arial", 10, XFontStyleEx.Regular);
        XBrush headerBrush = XBrushes.DarkBlue;
        XBrush subHeaderBrush = XBrushes.DimGray;
        XBrush textBrush = XBrushes.Black;

        // Title Section
        gfx.DrawString("Invoice Example", titleFont, XBrushes.DarkBlue,
            new XRect(40, 40, page.Width - 80, page.Height),
            XStringFormats.TopCenter);  // Center the title, make it dark blue for emphasis

        // Start Position
        double yPosition = 70;

        // Horizontal Line for Separation
        gfx.DrawLine(XPens.DarkGray, 40, yPosition, page.Width - 40, yPosition);

        // From Section
        yPosition += 20;
        gfx.DrawString("From:", subHeaderFont, XBrushes.Black,
            new XRect(60, yPosition, page.Width - 80, page.Height),
            XStringFormats.TopLeft);
        gfx.DrawString("Highway UHP", normalFont, XBrushes.Black,
            new XRect(60, yPosition + 20, page.Width - 80, page.Height),
            XStringFormats.TopLeft);
        gfx.DrawString("4501 S 2700 W, Salt Lake City, UT 84129", normalFont, XBrushes.Black,
            new XRect(60, yPosition + 40, page.Width - 80, page.Height),
            XStringFormats.TopLeft);
        gfx.DrawString("(801) 965-4518", normalFont, XBrushes.Black,
            new XRect(60, yPosition + 55, page.Width - 80, page.Height),
            XStringFormats.TopLeft);

        // For Section
        gfx.DrawString("For:", subHeaderFont, XBrushes.Black,
            new XRect(270, yPosition, page.Width - 80, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString("Company 123", normalFont, XBrushes.Black,
            new XRect(270, yPosition, page.Width - 80, page.Height),
            XStringFormats.TopLeft);

        // Horizontal Line
        yPosition += 70;
        gfx.DrawLine(XPens.DarkGray, 40, yPosition, page.Width - 40, yPosition);

        // Info Section
        yPosition += 20;
        gfx.DrawString("Invoice # 123456870", normalFont, XBrushes.Black,
            new XRect(60, yPosition, page.Width - 80, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString($"Date: {DateTime.Today.ToString("d")}", normalFont, XBrushes.Black,
            new XRect(60, yPosition, page.Width - 80, page.Height),
            XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString($"Due: {grandTotal:C}", normalFont, XBrushes.Black,
            new XRect(60, yPosition, page.Width - 80, page.Height),
            XStringFormats.TopLeft);

        // Horizontal Line
        yPosition += 30;
        gfx.DrawLine(XPens.DarkGray, 40, yPosition, page.Width - 40, yPosition);

        // Table Headers for Description, Hours, Rate
        yPosition += 20;
        gfx.DrawString("Description", subHeaderFont, XBrushes.Black,
            new XRect(40, yPosition, 200, page.Height),
            XStringFormats.TopLeft);
        gfx.DrawString("Hours", subHeaderFont, XBrushes.Black,
            new XRect(310, yPosition, 100, page.Height),
            XStringFormats.TopRight);
        gfx.DrawString("Rate($)", subHeaderFont, XBrushes.Black,
            new XRect(page.Width - 230, yPosition, 100, page.Height),
            XStringFormats.TopRight);
        gfx.DrawString("Amt($)", subHeaderFont, XBrushes.Black,
            new XRect(page.Width - 150, yPosition, 100, page.Height),
            XStringFormats.TopRight);

        // Horizontal Line
        yPosition += 20;
        gfx.DrawLine(XPens.DarkGray, 40, yPosition, page.Width - 40, yPosition);

        foreach (var data in invoicedata)
        {
            yPosition += 20;
            checkIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

            gfx.DrawString(data.projectId + " - " + data.projectName, headerFont, headerBrush,
                new XRect(40, yPosition, page.Width - 80, page.Height),
                XStringFormats.TopLeft);

            foreach (var shift in data.shiftsByProject)
            {
                yPosition += 20;
                checkIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

                gfx.DrawString("Shift: " + shift.shiftId + " - " + shift.shiftLocation, subHeaderFont, subHeaderBrush,
                    new XRect(60, yPosition, page.Width - 80, page.Height),
                    XStringFormats.TopLeft);

                foreach (var employee in shift.employeesByShift)
                {
                    yPosition += 20;
                    checkIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

                    gfx.DrawString($"Employee: {employee.employeeId} - {employee.employeeName}", normalFont, textBrush,
                        new XRect(80, yPosition, page.Width - 200, page.Height),
                        XStringFormats.TopLeft);

                    gfx.DrawString($"{employee.hoursWorked}", normalFont, textBrush,
                        new XRect(310, yPosition, 100, page.Height),
                        XStringFormats.TopRight);
                    gfx.DrawString("75", subHeaderFont, XBrushes.Black,
                        new XRect(page.Width - 230, yPosition, 100, page.Height),
                        XStringFormats.TopRight);
                    gfx.DrawString($"{employee.hoursWorked * 75}", subHeaderFont, XBrushes.Black,
                        new XRect(page.Width - 150, yPosition, 100, page.Height),
                        XStringFormats.TopRight);

                    hoursCounter += employee.hoursWorked;
                }
            }

            // Horizontal Line
            yPosition += 30;
            gfx.DrawLine(XPens.DarkGray, 40, yPosition, page.Width - 40, yPosition);
        }


        // Grand Total (Bottom Right)
        grandTotal = hoursCounter * 75;
        yPosition += 40;
        checkIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

        yPosition += 20;

        gfx.DrawString($"Grand Total: ", titleFont, XBrushes.Black,
            new XRect(60, yPosition, 200, 40),
            XStringFormats.TopLeft);
        gfx.DrawString($"{hoursCounter}", titleFont, XBrushes.Black,
            new XRect(310, yPosition, 100, page.Height),
            XStringFormats.TopRight);
        gfx.DrawString($"{grandTotal:C}", titleFont, XBrushes.Black,
            new XRect(page.Width - 150, yPosition, 100, page.Height),
            XStringFormats.TopRight);

        // Save the document
        var filename = "Invoice.pdf";
        document.Save(filename);

        var currentFilePath = System.IO.Path.GetFullPath(".");
        var fileBytes = System.IO.File.ReadAllBytes(currentFilePath + "/" + filename);

        if (System.IO.File.Exists(System.IO.Path.Combine(currentFilePath, filename)))
        {
            System.IO.File.Delete(System.IO.Path.Combine(currentFilePath, filename));
        }

        return File(fileBytes, "application/pdf", "Invoice.pdf");

    }

    private static void checkIfNewPageNeeded(int maxYPosition, PdfDocument document, ref PdfPage page, ref XGraphics gfx, ref double yPosition)
    {
        if (yPosition > maxYPosition)
        {
            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            yPosition = 40;
        }
    }
}
