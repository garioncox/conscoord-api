namespace conscoord_api.Data.DTOs;

public class EditEmployeeShiftDTO
{
    public int id { get; set; }
    public string clockInTime { get; set; }
    public string clockOutTime { get; set; }
    public int EmployeeId { get; set; }
    public int ShiftId { get; set; }
}
