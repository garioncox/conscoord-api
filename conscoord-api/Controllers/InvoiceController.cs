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
    private readonly IRoleUtils _RoleUtils;

    public InvoiceController(IInvoiceService invoiceService, IRoleUtils roleUtils)
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

        if (startDate > endDate)
        {
            return BadRequest("Please make sure the Start Date is before the End Date");
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

        if (startDate > endDate)
        {
            return BadRequest("Please make sure the Start Date is before the End Date");
        }

        double hoursCounter = 0;
        double grandTotal = 0;
        var maxYPosition = 750;
        var invoicedata = await interfaceService.GetInvoiceInfoByCompanyTimePeriod(DTO);
        if (invoicedata.Count == 0)
        {
            return BadRequest("There are no shifts in this time period");
        }

        Dictionary<int, double> projectGrandTotals = new Dictionary<int, double>();

        foreach (var data in invoicedata)
        {
            foreach (var shift in data.shiftsByProject)
            {
                foreach (var employee in shift.employeesByShift)
                {
                    grandTotal += employee.hoursWorked * 75;

                    if (projectGrandTotals.ContainsKey(data.projectId))
                    {
                        projectGrandTotals[data.projectId] += projectGrandTotals[data.projectId] + (employee.hoursWorked * 75);
                    }
                    else
                    {
                        projectGrandTotals[data.projectId] = employee.hoursWorked * 75;
                    }
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
#pragma warning disable CS0618 // Type or member is obsolete
        gfx.DrawString("Invoice Example", titleFont, XBrushes.DarkBlue,
            new XRect(40, 60, page.Width - 80, page.Height),
            XStringFormats.TopCenter);

        // Start Position
        double yPosition = 20;

        // From Section
        gfx.DrawString("Highway UHP", normalFont, XBrushes.Black,
            new XRect(10, yPosition - 10, page.Width - 80, page.Height),
            XStringFormats.TopLeft);
        gfx.DrawString("4501 S 2700 W, Salt Lake City, UT 84129", normalFont, XBrushes.Black,
            new XRect(10, yPosition + 5, page.Width - 80, page.Height),
            XStringFormats.TopLeft);
        gfx.DrawString("(801) 965-4518", normalFont, XBrushes.Black,
            new XRect(10, yPosition + 20, page.Width - 80, page.Height),
            XStringFormats.TopLeft);

        yPosition += 70;

        // Horizontal Line for Separation
        gfx.DrawLine(XPens.DarkGray, 40, yPosition, page.Width - 40, yPosition);

        // Info Section
        gfx.DrawString("Invoice # 123456870", normalFont, XBrushes.Black,
            new XRect(60, yPosition + 10, page.Width - 80, page.Height),
            XStringFormats.TopLeft);
        gfx.DrawString($"Date: {DateTime.Today.ToString("d")}", normalFont, XBrushes.Black,
            new XRect(60, yPosition + 30, page.Width - 80, page.Height),
            XStringFormats.TopLeft);
        gfx.DrawString($"Due: {grandTotal:C}", normalFont, XBrushes.Black,
            new XRect(60, yPosition + 50, page.Width - 80, page.Height),
            XStringFormats.TopLeft);

        // For Section
        gfx.DrawString("For:", subHeaderFont, XBrushes.Black,
            new XRect(270, yPosition + 10, page.Width - 80, page.Height),
            XStringFormats.TopLeft);
        gfx.DrawString("Company 123", normalFont, XBrushes.Black,
            new XRect(270, yPosition + 30, page.Width - 80, page.Height),
            XStringFormats.TopLeft);

        // Horizontal Line
        yPosition += 80;
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

                    gfx.DrawString($"{employee.hoursWorked:F2}", normalFont, textBrush,
                        new XRect(310, yPosition, 100, page.Height),
                        XStringFormats.TopRight);
                    gfx.DrawString("75", subHeaderFont, XBrushes.Black,
                        new XRect(page.Width - 230, yPosition, 100, page.Height),
                        XStringFormats.TopRight);
                    gfx.DrawString($"{employee.hoursWorked * 75:F2}", subHeaderFont, XBrushes.Black,
                        new XRect(page.Width - 150, yPosition, 100, page.Height),
                        XStringFormats.TopRight);

                    hoursCounter += employee.hoursWorked;
                }
            }

            yPosition += 20;

            gfx.DrawString("Project Total: ", subHeaderFont, XBrushes.Black,
                new XRect(page.Width - 220, yPosition + 5, 100, page.Height),
                XStringFormats.TopRight);

            gfx.DrawString($"{projectGrandTotals[data.projectId]:C}", subHeaderFont, XBrushes.Black,
                new XRect(page.Width - 150, yPosition + 5, 100, page.Height),
                XStringFormats.TopRight);

            // Horizontal Line
            yPosition += 40;
            gfx.DrawLine(XPens.DarkGray, 40, yPosition, page.Width - 40, yPosition);
        }


        // Grand Total (Bottom Right)
        yPosition += 25;
        checkIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

        gfx.DrawString($"Grand Total: ", titleFont, XBrushes.Black,
            new XRect(60, yPosition, 200, 40),
            XStringFormats.TopLeft);
        gfx.DrawString($"{hoursCounter:F2} hrs", titleFont, XBrushes.Black,
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
