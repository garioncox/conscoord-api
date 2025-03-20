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
    public TimeOnly? clockintime { get; set; }
    public TimeOnly? clockouttime { get; set; }
    public bool? is_residual { get; set; } = false;
    public int? invoiceId { get; set; } 
}

