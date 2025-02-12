namespace conscoord_api.Data.DTOs;
public class EmployeeShiftDTO
{
    public string? ClockInTime { get; set; }
    public string? ClockOutTime { get; set; }
    public int EmpId { get; set; }
    public int ShiftId { get; set; }
    public string? Notes { get; set; }
    public bool Hasbeeninvoiced { get; set; } = false;
    public bool Didnotwork { get; set; } = false;
    public bool Reportedcanceled { get; set; } = false;
}