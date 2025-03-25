namespace conscoord_api.Data.DTOs;

public class EmployeeHistoryDTO
{
    public required int shiftId { get; set; }
    public required string hours { get; set; }
    public required string location { get; set; }
    public required DateTime date { get; set; }
    public required string projectName { get; set; }
}
