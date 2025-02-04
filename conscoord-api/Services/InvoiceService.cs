using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;

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
        var allInvoiceInfo = _context.InvoiceData.FromSqlRaw
            (@"select p.id as projectId,p.""location"" as projectName,
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
where es.clock_in_time is not null and es.clock_out_time is not null;").AsNoTracking().ToList();

        List<InvoiceInfoDTO> result = new List<InvoiceInfoDTO>();
<<<<<<< HEAD
        Dictionary<int, int> projectIdToIndex = new Dictionary<int, int>();

        foreach (var row in allInvoiceInfo)
        {
            //check if project exists
            if (!projectIdToIndex.ContainsKey(row.projectId))
            {
                var employees = new List<employeeInfo> { new employeeInfo { employeeId = row.employeeId, employeeName = row.employeeName, employeePayRate = 75, hoursWorked = 8 } };
                var rowsShift = new shiftInfo { shiftId = row.shiftId, shiftLocation = row.shiftName, employeesByShift = employees };

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
                var rowsEmployee = new employeeInfo { employeeId = row.employeeId, employeeName = row.employeeName, employeePayRate = 75, hoursWorked = 8 };

                //check if the shift exists within the project
                var existingShift = project.shiftsByProject.FirstOrDefault(s => s.shiftId == row.shiftId);
                if (existingShift == null) 
                {
                    var employees = new List<employeeInfo> { new employeeInfo { employeeId = row.employeeId, employeeName = row.employeeName, employeePayRate = 75, hoursWorked = 8 } };
                    var rowsShift = new shiftInfo { shiftId = row.shiftId, shiftLocation = row.shiftName, employeesByShift = employees };
                    project.shiftsByProject.Add(rowsShift);
                }
                else 
                {
                    existingShift.employeesByShift.Add(new employeeInfo { employeeId = row.employeeId, employeeName = row.employeeName, employeePayRate = 75, hoursWorked = 8 });
                }
=======
        Dictionary<int, int> projects = new();
        Dictionary<int, int> shifts = new();
        foreach (var invoiceInfo in allInvoiceInfo)
        {
            var ClockOutParsed = DateTime.ParseExact(invoiceInfo.clockouttime, ["H:mm", "HH:mm"], null);
            var ClockInParsed = DateTime.ParseExact(invoiceInfo.clockintime, ["H:mm", "HH:mm"], null);
            var hoursWorked = ClockOutParsed - ClockInParsed;

            employeeInfo employee = new employeeInfo()
            {
                employeeId = invoiceInfo.employeeId,
                employeeName = invoiceInfo.employeeName,
                employeePayRate = invoiceInfo.payrate ?? 75,
                hoursWorked = (hoursWorked.TotalHours + 24) % 24
            };

            var shift = new shiftInfo()
            {
                shiftId = invoiceInfo.shiftId,
                shiftLocation = invoiceInfo.shiftName,
                employeesByShift = new()
            };

            if (!projects.ContainsKey(invoiceInfo.projectId))
            {
                var project = new InvoiceInfoDTO()
                {
                    projectId = invoiceInfo.projectId,
                    projectName = invoiceInfo.projectName,
                    shiftsByProject = new()
                };
                //todo check and add to shift dictionary
                project.shiftsByProject.Add(shift);
                //todo instaed of zero use the shift index
                project.shiftsByProject[0].employeesByShift.Add(employee);

                result.Add(project);
                projects[invoiceInfo.projectId] = result.Count()-1;
            }
            else
            {
                var projectindex = projects[invoiceInfo.projectId];
                result[projectindex].shiftsByProject.Add(shift);
                //todo check shift dictionary
                result[projectindex].shiftsByProject[0].employeesByShift.Add(employee);
>>>>>>> 2db86c6f35f6cf8a9b5d896f2a012750e2d7cd73
            }
        }

        return result;
    }
}
