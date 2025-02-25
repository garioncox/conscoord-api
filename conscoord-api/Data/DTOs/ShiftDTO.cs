namespace conscoord_api.Data.DTOs;

public class ShiftDTO
{
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public int RequestedEmployees { get; set; }
    public required string Status { get; set; }
}
