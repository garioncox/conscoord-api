namespace conscoord_api.Data.DTOs;
public class InvoiceFromDB
{
    public int projectId { get; set; }
    public DateTime shiftEnd { get; set; }
    public string projectName { get; set; } = null!;
    public int shiftId { get; set; }
    public string shiftName { get; set; } = null!;
    public int employeeId { get; set; }
    public string employeeName { get; set; } = null!;
    public decimal? payrate { get; set; } = 75;
    public TimeOnly? clockintime { get; set; } = null;
    public TimeOnly? clockouttime { get; set; } = null;
    public bool? has_been_invoiced { get; set; } = false;
}

