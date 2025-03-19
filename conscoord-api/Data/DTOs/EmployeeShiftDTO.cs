namespace conscoord_api.Data.DTOs;
public class EmployeeShiftDTO
{
    public int? Id { get; set; }
    public TimeOnly? ClockInTime { get; set; }
    public TimeOnly? ClockOutTime { get; set; }
    public bool Didnotwork { get; set; } = false;
    public int EmpId { get; set; }
    public bool? Hasbeeninvoiced { get; set; }
    public string? Notes { get; set; }
    public bool Reportedcanceled { get; set; } = false;
    public int ShiftId { get; set; }
}
