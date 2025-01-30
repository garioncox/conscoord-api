using System.Globalization;
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

    public async Task<List<InvoiceInfoDTO>> GetInvoiceInfoByCompanyTimePeriod(int companyId, DateTime startDate, DateTime endDate)
    {
        var allInvoiceInfo = _context.InvoiceData
            .FromSqlRaw(@"select p.id as projectId,p.""location"" as projectName,
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
where e.clockInTime is not null && e.clockOutTime is not null;").ToList();

        Console.WriteLine(allInvoiceInfo);
        //List<InvoiceInfoDTO> invoices = filteredProjects.Select(cp => new InvoiceInfoDTO
        //{
        //    projectId = cp.Id,
        //    projectName = cp.Name,
        //    shiftsByProject = cp.ProjectShifts
        //     .Where(ps => DateTime.TryParseExact(ps.Shift.EndTime, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endTime)
        //                  && endTime > startDate && endTime < endDate)

        //     .Select(ps => new shiftInfo
        //     {
        //         shiftId = ps.Shift.Id,
        //         shiftLocation = ps.Shift.Location,
        //         employeesByShift = ps.Shift.EmployeeShifts.Select(es =>
        //         {
        //             var ClockOutParsed = DateTime.ParseExact(es.ClockOutTime, ["H:mm", "HH:mm"], null);
        //             var ClockInParsed = DateTime.ParseExact(es.ClockInTime, ["H:mm", "HH:mm"], null);
        //             var hoursWorked = ClockOutParsed - ClockInParsed;

        //             return new employeeInfo
        //             {
        //                 employeeId = es.Emp.Id,
        //                 employeeName = es.Emp.Name,
        //                 employeePayRate = es.Emp.Payrate ?? 0,
        //                 hoursWorked = ((hoursWorked.TotalHours + 24) % 24)
        //             };
        //         }).ToList()
        //     }).ToList()
        //}).ToList();

        return new List<InvoiceInfoDTO>();
    }
}
