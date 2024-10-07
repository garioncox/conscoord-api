using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace practicum2425.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeShiftController(IEmployeeShiftService service, IShiftService shiftService) : ControllerBase
{
    private readonly IEmployeeShiftService _empShiftService = service;
    private readonly IShiftService _shiftService = shiftService;

    [HttpPost()]
    public async Task CreateEmpShift([FromBody] EmployeeShiftDTO empShift)
    {
        var signedUpFor = GetShiftsByEmpId(empShift.EmployeeId);
        var toSignUpFor = await _shiftService.GetShiftById(empShift.ShiftId);


        // For the shift we are adding, check to see if it overlaps with an existing shift we signed up for
        DateTime ts = DateTime.Parse(toSignUpFor.StartTime);
        DateTime te = DateTime.Parse(toSignUpFor.EndTime);
        foreach (Shift s in signedUpFor)
        {
            DateTime ss = DateTime.Parse(s.StartTime);
            DateTime se = DateTime.Parse(s.EndTime);
            if ((ts > ss && ts < se) || (te > ss && te < se))
            {
                return;
            }

            // Check to see if we are signing up for the same shift
            if (s.Id == toSignUpFor.Id)
            {
                return;
            }
        }

        EmployeeShift e = new()
        {
            EmpId = empShift.EmployeeId,
            ShiftId = empShift.ShiftId
        };

        await _empShiftService.CreateEmployeeShift(e);
    }

    [HttpDelete()]
    public async Task DeleteEmpShift(int empShiftId)
    {
        await _empShiftService.DeleteEmpShiftAsync(empShiftId);
    }

    [HttpGet("getShifts/{empId}")]
    public List<Shift> GetShiftsByEmpId(int empId)
    {
        return _empShiftService.GetScheduledShiftsByEmpId(empId);
    }
}