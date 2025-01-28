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
        var signedUpFor = _shiftService.GetScheduledShiftsByEmpId(empShift.EmployeeId);
        var toSignUpFor = await _shiftService.GetShiftById(empShift.ShiftId);
        if (toSignUpFor == null)
        {
            return NotFound();
        }

        // For the shift we are adding, check to see if it overlaps with an existing shift we signed up for
        DateTime ts = DateTime.Parse(toSignUpFor.StartTime);
        DateTime te = DateTime.Parse(toSignUpFor.EndTime);
        foreach (var s in signedUpFor)
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
            ShiftId = empShift.ShiftId,
            Notes = empShift.Notes,
        };

        await _empShiftService.CreateEmployeeShift(e);
        return Ok();
    }

    [HttpDelete("delete/{Id}")]
    public async Task DeleteEmpShift(int Id)
    {
        await _empShiftService.DeleteEmpShiftAsync(Id);
    }

    [HttpGet("getall")]
    public List<EmployeeShift> GetAllShifts()
    {
        return _empShiftService.GetallEmployeeShifts();
    }

    [HttpGet("get/{email}")]
    public async Task<List<EmployeeShift>> GetByEmail(string email)
    {
        return await _empShiftService.GetEmployeeShiftsByEmail(email);
    }

    [HttpPut("edit")]
    public Task UpdateEmpShift([FromBody] EditEmployeeShiftDTO updatedEmpShift)
    {
        return _empShiftService.UpdateEmpShift(updatedEmpShift);
    }

    [HttpGet("get/history/{email}")]

    public async Task<IActionResult> GetHistoryByEmail(string email)
    {
        var result =  await _empShiftService.GetHistoryByEmail(email);
        Console.WriteLine($"controller: {result[0].location}");

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
