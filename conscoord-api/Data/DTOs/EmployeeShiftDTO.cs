namespace conscoord_api.Data.DTOs;
public class EmployeeShiftDTO
{
    public int EmployeeId { get; set; }
    public int ShiftId { get; set; }
    public string? Notes { get; set; } = null;
}
