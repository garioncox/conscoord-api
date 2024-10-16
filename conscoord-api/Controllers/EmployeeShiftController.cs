using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeShiftController(IEmployeeShiftService service, IShiftService shiftService) : ControllerBase
{
    private readonly IEmployeeShiftService _empShiftService = service;
    private readonly IShiftService _shiftService = shiftService;

    [HttpPost("add")]
    public async Task<ActionResult> CreateEmpShift([FromBody] EmployeeShiftDTO empShift)
    {
        var signedUpFor = GetShiftsByEmpId(empShift.EmployeeId);
        var toSignUpFor = await _shiftService.GetShiftById(empShift.ShiftId);
        if (toSignUpFor == null)
        {
            return NotFound();
        }

        // For the shift we are adding, check to see if it overlaps with an existing shift we signed up for
        DateTime ts = DateTime.Parse(toSignUpFor.StartTime);
        DateTime te = DateTime.Parse(toSignUpFor.EndTime);
        foreach (Shift s in signedUpFor)
        {
            DateTime ss = DateTime.Parse(s.StartTime);
            DateTime se = DateTime.Parse(s.EndTime);
            if (ts > ss && ts < se || te > ss && te < se)
            {
                return StatusCode(500);
            }

            // Check to see if we are signing up for the same shift
            if (s.Id == toSignUpFor.Id)
            {
                return StatusCode(500);
            }
        }

        EmployeeShift e = new()
        {
            EmpId = empShift.EmployeeId,
            ShiftId = empShift.ShiftId
        };

        await _empShiftService.CreateEmployeeShift(e);
        return Ok();
    }

    [HttpDelete("delete/{Id}")]
    public async Task DeleteEmpShift(int Id)
    {
        await _empShiftService.DeleteEmpShiftAsync(Id);
    }

    [HttpGet("getShifts/{empId}")]
    public List<Shift> GetShiftsByEmpId(int empId)
    {
        return _empShiftService.GetScheduledShiftsByEmpId(empId);
    }

    [HttpGet("get/{email}")]
    public List<Shift> getSignedUpShift(string email)
    {
        return _empShiftService.getSignedUpShift(email);
    }
}