namespace conscoord_api.Data.DTOs;
public class InvoiceFromDB
{
    public int projectId { get; set; }
    public string projectName { get; set; } = null!;
    public int shiftId { get; set; }
    public string shiftName { get; set; } = null!;
    public int employeeId { get; set; }
    public string employeeName { get; set; } = null!;
    public decimal? payrate { get; set; } = 75;
    public string? clockintime { get; set; } = null;
    public string? clockouttime { get; set; } = null;
}

