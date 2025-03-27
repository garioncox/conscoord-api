using System.Collections;
using System.Text.Json.Serialization;
using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using conscoord_api.Services;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly IRoleUtils _RoleUtils;
    private readonly AzureFileService _FilesService;

    public InvoiceController(IInvoiceService invoiceService, IRoleUtils roleUtils, AzureFileService filesService)
    {
        _invoiceService = invoiceService;
        _RoleUtils = roleUtils;
        _FilesService = filesService;
    }

    [HttpPost("getInvoicePreview")]
    public async Task<ActionResult<List<InvoiceInfoDTO>>> GetInvoiceInfoByCompanyTimePeriod(InvoiceDTO DTO)
    {
        var user = HttpContext.User;
        var hasPerms = await _RoleUtils.HasPerms(user, [Role.ADMIN_ROLE]);
        if (!hasPerms) { return NotFound(); }

        if (DTO.startDate > DTO.endDate) { return BadRequest("Selected Start Date cannot be after End Date"); }

        var result = await _invoiceService.GetInvoiceInfoByCompanyTimePeriod(DTO);
        return Ok(result);
    }

    [HttpPost("generateInvoice")]
    public async Task<IActionResult> GeneratePDF(IInvoiceService interfaceService, InvoiceDTO DTO, ICompanyService companyService)
    {
        var user = HttpContext.User;
        var hasPerms = await _RoleUtils.HasPerms(user, [Role.ADMIN_ROLE]);
        if (!hasPerms) { return NotFound(); }

        if (DTO.startDate > DTO.endDate)
        {
            return BadRequest("Please make sure the Start Date is before the End Date");
        }

        double hoursCounter = 0;
        double grandTotal = 0;
        var maxYPosition = 750;
        var invoicedata = await interfaceService.GetInvoiceInfoByCompanyTimePeriod(DTO);

        List<InvoiceInfoDTO> residualShifts = new List<InvoiceInfoDTO>();

        //Remove data with errors
        foreach (var project in invoicedata.ToList())
        {
            foreach (var shift in project.shiftsByProject.ToList())
            {
                // Remove employees with hoursWorked == 0
                shift.employeesByShift = shift.employeesByShift
                    .Where(emp => emp.hoursWorked > 0)
                    .ToList();

                // Remove shift if it has no employees left
                if (!shift.employeesByShift.Any())
                {
                    project.shiftsByProject.Remove(shift);
                }
            }

            // Remove project if it has no shifts left
            if (!project.shiftsByProject.Any())
            {
                invoicedata.Remove(project);
            }
        }

        //Separate out the residual shifts
        foreach (var project in invoicedata.ToList())
        {
            // Create a new project DTO for residual shifts
            var residualProject = new InvoiceInfoDTO
            {
                projectId = project.projectId,
                projectName = project.projectName,
                shiftsByProject = new List<shiftInfo>()
            };

            foreach (var shift in project.shiftsByProject.ToList())
            {
                // Separate residual employees
                var shiftResidualEmployees = shift.employeesByShift
                    .Where(emp => emp.is_residual == true)
                    .ToList();

                if (shiftResidualEmployees.Any())
                {
                    // Add to the residual project
                    residualProject.shiftsByProject.Add(new shiftInfo
                    {
                        shiftId = shift.shiftId,
                        shiftLocation = shift.shiftLocation,
                        employeesByShift = shiftResidualEmployees
                    });

                    // Remove residual employees from the original shift
                    shift.employeesByShift = shift.employeesByShift
                        .Where(emp => emp.is_residual != true)
                        .ToList();
                }

                // Remove shifts with no employees left
                if (!shift.employeesByShift.Any())
                {
                    project.shiftsByProject.Remove(shift);
                }
            }

            // Only add the project if it contains residual shifts
            if (residualProject.shiftsByProject.Any())
            {
                residualShifts.Add(residualProject);
            }
        }

        // Remove projects that have no shifts left
        invoicedata = invoicedata
            .Where(project => project.shiftsByProject.Any())
            .ToList();

        var companies = await companyService.GetCompanyListAsync();
        var company = companies.Where(c => c.Id == DTO.companyId).FirstOrDefault();

        if (company is null)
        {
            return BadRequest("Ensure that a company is passed in");
        }

        if (invoicedata.Count == 0 && residualShifts.Count == 0)
        {
            return BadRequest("There are no valid shifts in this time period");
        }

        Dictionary<(int, bool), double> projectGrandTotals = new Dictionary<(int, bool), double>();
        List<InvoiceInfoDTO> itemsToRemove = new List<InvoiceInfoDTO>();
        List<ShiftDTO> shiftsToRemove = new List<ShiftDTO>();
        List<EmployeeDTO> employeesToRemove = new List<EmployeeDTO>();

        //Calculate project totals and grand total
        foreach (var data in invoicedata.ToList())
        {
            foreach (var shift in data.shiftsByProject)
            {
                foreach (var employee in shift.employeesByShift)
                {
                    grandTotal += employee.hoursWorked * 75;

                    if (projectGrandTotals.ContainsKey((data.projectId, false)))
                    {
                        projectGrandTotals[(data.projectId, false)] = projectGrandTotals[(data.projectId, false)] + (employee.hoursWorked * 75);
                    }
                    else
                    {
                        projectGrandTotals[(data.projectId, false)] = employee.hoursWorked * 75;
                    }
                }
            }
        }

        //get project totals for the residual shifts
        foreach (var data in residualShifts.ToList())
        {
            foreach (var shift in data.shiftsByProject)
            {
                foreach (var employee in shift.employeesByShift)
                {
                    grandTotal += employee.hoursWorked * 75;

                    if (projectGrandTotals.ContainsKey((data.projectId, true)))
                    {
                        projectGrandTotals[(data.projectId, true)] = projectGrandTotals[(data.projectId, true)] + (employee.hoursWorked * 75);
                    }
                    else
                    {
                        projectGrandTotals[(data.projectId, true)] = employee.hoursWorked * 75;
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
        gfx.DrawString(company.Name, normalFont, XBrushes.Black,
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
            CheckIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

            gfx.DrawString(data.projectId + " - " + data.projectName, headerFont, headerBrush,
                new XRect(40, yPosition, page.Width - 80, page.Height),
                XStringFormats.TopLeft);

            foreach (var shift in data.shiftsByProject)
            {

                yPosition += 20;
                CheckIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

                gfx.DrawString("Shift: " + shift.shiftId + " - " + shift.shiftLocation, subHeaderFont, subHeaderBrush,
                    new XRect(60, yPosition, page.Width - 80, page.Height),
                    XStringFormats.TopLeft);

                foreach (var employee in shift.employeesByShift)
                {
                    await interfaceService.updateHasBeenInvoiced(employee, shift);

                    yPosition += 20;
                    CheckIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

                    gfx.DrawString($"Employee: {employee.employeeId} - {employee.employeeName}", normalFont, textBrush,
                        new XRect(80, yPosition, page.Width - 200, page.Height),
                        XStringFormats.TopLeft);

                    gfx.DrawString($"{employee.hoursWorked:F2}", normalFont, textBrush,
                        new XRect(310, yPosition, 100, page.Height),
                        XStringFormats.TopRight);
                    gfx.DrawString("75", normalFont, XBrushes.Black,
                        new XRect(page.Width - 230, yPosition, 100, page.Height),
                        XStringFormats.TopRight);
                    gfx.DrawString($"{employee.hoursWorked * 75:F2}", normalFont, XBrushes.Black,
                        new XRect(page.Width - 150, yPosition, 100, page.Height),
                        XStringFormats.TopRight);

                    textBrush = XBrushes.Black;

                    hoursCounter += employee.hoursWorked;
                }
            }

            yPosition += 20;

            gfx.DrawString("Project Total: ", subHeaderFont, XBrushes.Black,
                new XRect(page.Width - 220, yPosition + 5, 100, page.Height),
                XStringFormats.TopRight);

            gfx.DrawString($"{projectGrandTotals[(data.projectId, false)]:C}", subHeaderFont, XBrushes.Black,
                new XRect(page.Width - 150, yPosition + 5, 100, page.Height),
                XStringFormats.TopRight);

            // Horizontal Line
            yPosition += 40;
            gfx.DrawLine(XPens.DarkGray, 40, yPosition, page.Width - 40, yPosition);
        }

        gfx.DrawString("Residual Shifts", titleFont, XBrushes.Black,
        new XRect(page.Width / 2 - 30, yPosition + 5, 100, page.Height),
        XStringFormats.TopRight);

        // Horizontal Line
        yPosition += 40;
        gfx.DrawLine(XPens.DarkGray, 40, yPosition, page.Width - 40, yPosition);

        foreach (var data in residualShifts)
        {
            yPosition += 20;
            CheckIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

            gfx.DrawString(data.projectId + " - " + data.projectName, headerFont, headerBrush,
                new XRect(40, yPosition, page.Width - 80, page.Height),
                XStringFormats.TopLeft);

            foreach (var shift in data.shiftsByProject)
            {

                yPosition += 20;
                CheckIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

                gfx.DrawString("Shift: " + shift.shiftId + " - " + shift.shiftLocation, subHeaderFont, subHeaderBrush,
                    new XRect(60, yPosition, page.Width - 80, page.Height),
                    XStringFormats.TopLeft);

                foreach (var employee in shift.employeesByShift)
                {
                    await interfaceService.updateHasBeenInvoiced(employee, shift);

                    yPosition += 20;
                    CheckIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

                    gfx.DrawString($"Employee: {employee.employeeId} - {employee.employeeName}", normalFont, textBrush,
                        new XRect(80, yPosition, page.Width - 200, page.Height),
                        XStringFormats.TopLeft);

                    gfx.DrawString($"{employee.hoursWorked:F2}", normalFont, textBrush,
                        new XRect(310, yPosition, 100, page.Height),
                        XStringFormats.TopRight);
                    gfx.DrawString("75", normalFont, XBrushes.Black,
                        new XRect(page.Width - 230, yPosition, 100, page.Height),
                        XStringFormats.TopRight);
                    gfx.DrawString($"{employee.hoursWorked * 75:F2}", normalFont, XBrushes.Black,
                        new XRect(page.Width - 150, yPosition, 100, page.Height),
                        XStringFormats.TopRight);

                    textBrush = XBrushes.Black;

                    hoursCounter += employee.hoursWorked;
                }
            }

            yPosition += 20;

            gfx.DrawString("Project Total: ", subHeaderFont, XBrushes.Black,
                new XRect(page.Width - 220, yPosition + 5, 100, page.Height),
                XStringFormats.TopRight);

            gfx.DrawString($"{projectGrandTotals[(data.projectId, true)]:C}", subHeaderFont, XBrushes.Black,
                new XRect(page.Width - 150, yPosition + 5, 100, page.Height),
                XStringFormats.TopRight);

            // Horizontal Line
            yPosition += 40;
            gfx.DrawLine(XPens.DarkGray, 40, yPosition, page.Width - 40, yPosition);
        }

        // Grand Total (Bottom Right)
        yPosition += 25;
        CheckIfNewPageNeeded(maxYPosition, document, ref page, ref gfx, ref yPosition);

        gfx.DrawString($"Grand Total: ", titleFont, XBrushes.Black,
            new XRect(60, yPosition, 200, 40),
            XStringFormats.TopLeft);
        gfx.DrawString($"{hoursCounter:F2} hrs", titleFont, XBrushes.Black,
            new XRect(310, yPosition, 100, page.Height),
            XStringFormats.TopRight);
        gfx.DrawString($"{grandTotal:C}", titleFont, XBrushes.Black,
            new XRect(page.Width - 150, yPosition, 100, page.Height),
            XStringFormats.TopRight);


        var filename = $"Invoice.pdf";
        document.Save(filename);

        var currentFilePath = System.IO.Path.GetFullPath(".");
        var fileBytes = System.IO.File.ReadAllBytes(currentFilePath + "/" + filename);

        // Save the document
        if (System.IO.File.Exists(System.IO.Path.Combine(currentFilePath, filename)))
        {
            System.IO.File.Delete(System.IO.Path.Combine(currentFilePath, filename));
        }

        var InvoiceCreated = DateTime.Now;
        var name = FormatInvoiceName(company.Name, DTO.startDate.Date.ToString(), DTO.endDate.Date.ToString());

        var AzureResponse = await UploadToAzure(fileBytes, name);

        if (AzureResponse.Blob.URI is null)
        {
            Console.WriteLine(AzureResponse.Status);
            return BadRequest("No URL returned from Azure, a file of the same name likely exists");
        }
        if (AzureResponse.Error)
        {
            return BadRequest("Error uploading to Azure: " + AzureResponse.Status);
        }

        var invoice = await _invoiceService.CreateInvoice(DTO.companyId,InvoiceCreated);

        await _invoiceService.AddURL(invoice.Id, AzureResponse.Blob.URI);

        return File(fileBytes, "application/pdf", filename);
    }

    private async Task<AzureResponseDTO> UploadToAzure(byte[] fileBytes, string name)
    {
        var stream = new MemoryStream(fileBytes);
        IFormFile file = new FormFile(stream, 0, fileBytes.Length, name, name);
        var uploadResponse = await _FilesService.uploadAsync(file);
        return uploadResponse;
    }

    private static string FormatInvoiceName(string companyNameRaw, string startDate, string endDate)
    {
        var companyName = companyNameRaw.Replace(" ", "-");
        var name = $"Invoice_{companyName}_{startDate}_{endDate}.pdf";

        return name
            .Replace(" 12:00:00 AM", "")
            .Replace(" ", "_")
            .Replace("/", "-");
    }

    private static void CheckIfNewPageNeeded(int maxYPosition, PdfDocument document, ref PdfPage page, ref XGraphics gfx, ref double yPosition)
    {
        if (yPosition > maxYPosition)
        {
            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            yPosition = 40;
        }
    }
}
