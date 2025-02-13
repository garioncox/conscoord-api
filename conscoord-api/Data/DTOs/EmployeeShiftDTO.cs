namespace conscoord_api.Data.DTOs;
public class EmployeeShiftDTO
{
    public int? Id { get; set; }
    public string? ClockInTime { get; set; }
    public string? ClockOutTime { get; set; }
    public bool? Didnotwork { get; set; }
    public int EmpId { get; set; }
    public bool? Hasbeeninvoiced { get; set; }
    public string? Notes { get; set; }
    public bool? Reportedcanceled { get; set; }
    public int ShiftId { get; set; }
}