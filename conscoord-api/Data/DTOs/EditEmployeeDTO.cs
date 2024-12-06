namespace conscoord_api.Data.DTOs;

public class EditEmployeeShiftDTO
{
    public int id { get; set; }
    public required string clockInTime { get; set; }
    public required string clockOutTime { get; set; }
    public int EmployeeId { get; set; }
    public int ShiftId { get; set; }
    public string? Notes { get; set; } = null;
}
