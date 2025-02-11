using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace conscoord_api.Services;

public class InvoiceService : IInvoiceService
{
    readonly PostgresContext _context;

    public InvoiceService(PostgresContext context)
    {
        _context = context;
    }

    public async Task<List<InvoiceInfoDTO>> GetInvoiceInfoByCompanyTimePeriod(InvoiceDTO DTO)
    {
        var startDateValidate = DateTime.TryParseExact(DTO.startDate, ["yyyy/MM/dd", "yyyy/MM/d"], null, System.Globalization.DateTimeStyles.None, out var startDate);
        var endDateValidate = DateTime.TryParseExact(DTO.endDate, ["yyyy/MM/dd", "yyyy/MM/d"], null, System.Globalization.DateTimeStyles.None, out var endDate);

        var allInvoiceInfo = _context.InvoiceData.FromSqlRaw
            (@"select p.id as projectId,p.""location"" as projectName, s.end_time as shiftEnd,
                s.id as shiftId,s.""location"" as shiftName, 
                e.id as employeeId, e.name as employeeName, e.payrate, es.clock_in_time as clockInTime, es.clock_out_time as clockOutTime
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
                where es.clock_in_time is not null and es.clock_out_time is not null and cp.company_id = {0};", DTO.companyId)
            .AsNoTracking().ToList();

        List<InvoiceInfoDTO> result = new List<InvoiceInfoDTO>();
        Dictionary<int, int> projectIdToIndex = new Dictionary<int, int>();

        foreach (var row in allInvoiceInfo)
        {
            var ClockOutParsed = DateTime.ParseExact(row.clockouttime, ["H:mm", "HH:mm"], null);
            var ClockInParsed = DateTime.ParseExact(row.clockintime, ["H:mm", "HH:mm"], null);
            var hoursSpan = ClockOutParsed - ClockInParsed;
            var hoursWorked = (hoursSpan.TotalHours + 24) % 24;

            var endDateValid = DateTime.TryParseExact(row.shiftEnd, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var shiftEndDate);

            if (shiftEndDate < startDate || shiftEndDate > endDate)
            {
                continue;
            }

            var rowsEmployee = new employeeInfo { employeeId = row.employeeId, employeeName = row.employeeName, employeePayRate = row.payrate ?? 75, hoursWorked = hoursWorked };
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
}
