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
    public ShiftController(IShiftService service)
    {
        _shiftService = service;
    }

    [HttpGet("getAll")]
    public async Task<List<Shift>> GetShiftsListAsync()
    {
        return await _shiftService.GetAllShifts();
    }

    [HttpGet("get/{shiftId}")]
    public async Task<Shift> GetShiftByIdAsync(int shiftId) =>
        await _shiftService.GetShiftById(shiftId);


    [HttpGet("getAll/archived")]
    public async Task<List<Shift>> GetArchivedAndCompletedShiftsAsync()
    {
        return await _shiftService.GetAllArchivedAndCompletedShifts();
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

    [HttpDelete("delete/{id}")]
    public async Task Delete(int id)
    {
        await _shiftService.DeleteShiftAsync(id);
    }
}
