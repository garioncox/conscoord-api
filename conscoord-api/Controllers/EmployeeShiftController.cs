using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using conscoord_api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeShiftController(IEmployeeShiftService service, IShiftService shiftService, IRoleUtils roleUtils) : ControllerBase
{
    private readonly IEmployeeShiftService _empShiftService = service;
    private readonly IShiftService _shiftService = shiftService;
    private readonly IRoleUtils _roleUtils = roleUtils;

    [HttpPost("add")]
    public async Task<ActionResult> CreateEmpShift([FromBody] EmployeeShiftDTO empShift)
    {
        var user = HttpContext.User;
        var hasPerms = await _roleUtils.HasPerms(user, Role.ALL_ROLES);
        if (!hasPerms) { return BadRequest("User not be logged in"); }

        var signedUpFor = _shiftService.GetScheduledShiftsByEmpId(empShift.EmpId);
        var toSignUpFor = await _shiftService.GetShiftById(empShift.ShiftId);
        if (toSignUpFor == null)
        {
            return NotFound();
        }

        // For the shift we are adding, check to see if it overlaps with an existing shift we signed up for
        foreach (var s in signedUpFor)
        {
            if (toSignUpFor.StartTime > s.StartTime && toSignUpFor.StartTime < s.EndTime ||
                toSignUpFor.EndTime > s.StartTime && toSignUpFor.EndTime < s.EndTime)
            {
                return BadRequest($"Shift overlaps with existing shift [{s.Id}]");
            }

            // Check to see if we are signing up for the same shift
            if (s.Id == toSignUpFor.Id)
            {
                return StatusCode(500);
            }
        }

        EmployeeShift e = new()
        {
            EmpId = empShift.EmpId,
            ShiftId = empShift.ShiftId,
            DidNotWork = false,
            ReportedCanceled = false
        };

        await _empShiftService.CreateEmployeeShift(e);
        return Ok();
    }

    [HttpDelete("delete/{Id}")]
    public async Task DeleteEmpShiftByShiftId(int Id)
    {
        await _empShiftService.DeleteEmpShiftByShiftIdAsync(Id);
    }

    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteEmpShift(int shiftId, int employeeId)
    {
        try
        {
            var deleted = await _empShiftService.DeleteEmpShiftByShiftIdAndEmployeeIdAsync(shiftId, employeeId);
            if (!deleted)
            {
                return BadRequest("Employee Not Connected To Shift.");
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
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
    public async Task UpdateEmpShift([FromBody] EmployeeShiftDTO updatedEmpShift)
    {
        var user = HttpContext.User;
        var hasPerms = await _roleUtils.HasPerms(user, Role.ALL_ROLES);
        if (!hasPerms) { return; }

        await _empShiftService.UpdateEmpShift(updatedEmpShift);
    }

    [HttpGet("get/history/{email}")]
    public async Task<IActionResult> GetHistoryByEmail(string email)
    {
        var result = await _empShiftService.GetHistoryByEmail(email);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
