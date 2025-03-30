using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace conscoord_api.Services;

public class InvoiceService : IInvoiceService
{
    readonly PostgresContext _context;

    public InvoiceService(PostgresContext context)
    {
        _context = context;
    }

    public async Task<Invoice> CreateInvoice(int CompanyId, string Url)
    {
        Guid guid = Guid.NewGuid();
        var invoice = new Invoice
        {
            CompanyId = CompanyId,
            InvoiceNumber = guid,
            InvoiceUrl = Url,
            PostedDate = DateTime.Now.ToUniversalTime()
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        return invoice;
    }

    public async Task<List<InvoiceInfoDTO>> GetInvoiceInfoByCompanyTimePeriod(InvoiceDTO DTO)
    {
        var SQLQuery = @$"select p.id as projectId,p.""location"" as projectName, s.end_time as shiftEnd,
                s.id as shiftId,s.""location"" as shiftName, 
                e.id as employeeId, e.name as employeeName, e.payrate, es.clock_in_time as clockInTime, es.clock_out_time as clockOutTime, es.invoice_id as InvoiceId, es.is_residual
                from practicum2425.project p
                join practicum2425.company_project cp
                on cp.project_id = p.id
                join practicum2425.project_shift ps
                on ps.project_id = p.id
                join practicum2425.shift s
                on s.id = ps.shift_id
                join practicum2425.employee_shift es
                on es.shift_id = s.id
                join practicum2425.employee e
                on e.id = es.emp_id
                where cp.company_id = {DTO.companyId} and es.invoice_id is null;";

        var allInvoiceInfo = _context.InvoiceData.FromSqlRaw(SQLQuery)
                .AsNoTracking()
                .ToList();

        List<InvoiceInfoDTO> result = new List<InvoiceInfoDTO>();
        Dictionary<int, int> projectIdToIndex = new Dictionary<int, int>();


        foreach (var row in allInvoiceInfo)
        {
            var ClockOutParsed = row.clockouttime;
            var ClockInParsed = row.clockintime;
            var hoursSpan = ClockOutParsed - ClockInParsed;
            var hoursWorked = 0.0;
            if (hoursSpan is not null)
            {
                hoursWorked = hoursSpan.Value.TotalHours;
            }

            var shiftEndDate = row.shiftEnd;

            //include residual shifts means anything in the past will show up
            if (DTO.includeResidualShifts)
            {
                if (row.is_residual == null || (bool)!row.is_residual)
                {
                    if (shiftEndDate < DTO.startDate || shiftEndDate > DTO.endDate)
                    {
                        continue;
                    }
                }
            }
            else
            {
                if (shiftEndDate < DTO.startDate || shiftEndDate > DTO.endDate)
                {
                    continue;
                }
            }
            var rowsEmployee = new employeeInfo { employeeId = row.employeeId, employeeName = row.employeeName, employeePayRate = row.payrate ?? 75, hoursWorked = hoursWorked, invoiceId = row.invoiceId, is_residual = row.is_residual };

            var employees = new List<employeeInfo> { rowsEmployee };
            var rowsShift = new shiftInfo { shiftId = row.shiftId, shiftLocation = row.shiftName, employeesByShift = employees };

            //check if project exists
            if (!projectIdToIndex.ContainsKey(row.projectId))
            {
                var project = new InvoiceInfoDTO
                {
                    projectId = row.projectId,
                    projectName = row.projectName,
                    shiftsByProject = new List<shiftInfo> { rowsShift }
                };

                result.Add(project);
                projectIdToIndex[row.projectId] = result.Count - 1;
            }
            else
            {
                var project = result[projectIdToIndex[row.projectId]];

                //check if the shift exists within the project
                var existingShift = project.shiftsByProject.FirstOrDefault(s => s.shiftId == row.shiftId);
                if (existingShift is null)
                {
                    project.shiftsByProject.Add(rowsShift);
                }
                else
                {
                    existingShift.employeesByShift.Add(rowsEmployee);
                }
            }
        }
        await Task.CompletedTask;
        return result;
    }

    public async Task updateHasBeenInvoiced(employeeInfo rowsEmployee, shiftInfo rowsShift, int invoiceId)
    {
        var dbEmpShift = await _context.EmployeeShifts.FirstOrDefaultAsync(es => es.EmpId == rowsEmployee.employeeId && es.ShiftId == rowsShift.shiftId);

        if (dbEmpShift is not null)
        {
            dbEmpShift.InvoiceId = invoiceId;
            await _context.SaveChangesAsync();
        }
    }

    public async Task markAsResidual(int rowsEmployee, int rowsShift)
    {
        var dbEmpShift = await _context.EmployeeShifts.FirstOrDefaultAsync(es => es.EmpId == rowsEmployee && es.ShiftId == rowsShift);

        if (dbEmpShift is not null)
        {
            dbEmpShift.IsResidual = true;
            await _context.SaveChangesAsync();
        }
    }
    public async Task<List<AzureInvoiceDTO>> GetAllInvoicesByCompany(int companyId)
    {
        var invoices = await _context.Invoices
            .Where(i => i.CompanyId == companyId)
            .ToListAsync();

        if (invoices.IsNullOrEmpty())
        {
            return [];
        }

        return invoices.Select(i => new AzureInvoiceDTO()
        {
            Id = i.InvoiceNumber,
            URI = i.InvoiceUrl,
            Name = string.IsNullOrEmpty(i.InvoiceUrl) ? "" : Path.GetFileNameWithoutExtension(i.InvoiceUrl)
        }).ToList();
    }
}
