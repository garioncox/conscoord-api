using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftController : ControllerBase
{
    private readonly IShiftService _shiftService;
    private readonly IEmployeeShiftService _employeeShiftService;
    private readonly IRoleUtils _roleUtils;
    public ShiftController(IRoleUtils roleUtils, IShiftService service, IEmployeeShiftService employeeShiftService)
    {
        _shiftService = service;
        _employeeShiftService = employeeShiftService;
        _roleUtils = roleUtils;
    }

    [HttpGet("getAll")]
    public async Task<List<Shift>> GetShiftsListAsync()
    {
        return await _shiftService.GetAllShifts();
    }

    [HttpGet("getAll/errored/{companyId}")]
    public async Task<ActionResult<List<Shift>>> GetShiftsWithErrors(int companyId)
    {
        var user = HttpContext.User;
        var hasPerms = await _roleUtils.HasPerms(user, [Role.ADMIN_ROLE]);
        if (!hasPerms) { return BadRequest("User is not logged in"); }

        var shifts = await _shiftService.GetShiftsWithErrorsByCompany(companyId);

        return Ok(shifts);
    }

    [HttpGet("getAll/errored/dates/{companyId}")]
    public async Task<ActionResult<List<string>>> GetDatesWithErrors(int companyId)
    {
        var user = HttpContext.User;
        var hasPerms = await _roleUtils.HasPerms(user, [Role.ADMIN_ROLE]);
        if (!hasPerms) { return BadRequest("User is not logged in"); }

        var result = await GetShiftsWithErrors(companyId);
        if (result.Result is not OkObjectResult okResult)
        {
            return BadRequest("Failed to fetch shifts with errors.");
        }

        if (okResult.Value is not List<Shift> shiftsList)
        {
            return BadRequest("Invalid data returned from GetShiftsWithErrors.");
        }

        var errorDates = shiftsList
            .Select(s => s.StartTime.Date)
            .Distinct()
            .ToList();

        return Ok(errorDates);
    }

    [HttpGet("get/{shiftId}")]
    public async Task<ActionResult<Shift>> GetShiftByIdAsync(int shiftId)
    {
        var shift = await _shiftService.GetShiftById(shiftId);
        if (shift == null) { return NotFound(); }
        return shift;

    }

    [HttpGet("getByEmail/{email}")]
    public List<Shift> getSignedUpShift(string email)
    {
        return _shiftService.GetScheduledShiftsByEmail(email);
    }

    [HttpGet("getAll/archived")]
    public async Task<List<Shift>> GetArchivedAndCompletedShiftsAsync()
    {
        return await _shiftService.GetAllArchivedAndCompletedShifts();
    }

    [HttpGet("getAll/projectId/{projectId}")]
    public async Task<List<Shift>> GetShiftsByProject(int projectId)
    {
        return await _shiftService.GetShiftsByProject(projectId);
    }

    [HttpPost("add")]
    public async Task<int> CreateShift([FromBody] ShiftDTO shiftDTO)
    {
        Shift shift = new()
        {
            StartTime = shiftDTO.StartTime,
            EndTime = shiftDTO.EndTime,
            Description = shiftDTO.Description,
            Location = shiftDTO.Location,
            RequestedEmployees = shiftDTO.RequestedEmployees,
            Status = Shift.STATUS_ACTIVE,
        };

        await _shiftService.CreateShift(shift);
        return shift.Id;
    }

    [HttpPut("archive/{shiftId}")]
    public async Task ArchiveShift(int shiftId)
    {
        await _shiftService.ArchiveShiftAsync(shiftId);
    }

    [HttpPut("edit")]
    public async Task EditShift([FromBody] Shift shift)
    {
        await _shiftService.EditShiftAsync(shift);
    }
}
