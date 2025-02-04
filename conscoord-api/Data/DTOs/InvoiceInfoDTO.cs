namespace conscoord_api.Data.DTOs;

public class InvoiceInfoDTO
{
    public int projectId { get; set; }
    public string projectName { get; set; }
    public List<shiftInfo> shiftsByProject { get; set; }
}

public class shiftInfo
{
    public int shiftId { get; set; }
    public string shiftLocation { get; set; }
    public List<employeeInfo> employeesByShift { get; set; }
}

public class employeeInfo
{
    public int employeeId { get; set; }
    public string employeeName { get; set; }
    public decimal employeePayRate { get; set; }
    public double hoursWorked { get; set; }
}

public class InvoiceFromDB
{
    public int projectId { get; set; }
    public string projectName { get; set; } = null!;
    public string employeeName { get; set; } = null!;
    public int shiftId { get; set; }
    public string shiftName { get; set; } = null!;
    public int employeeId { get; set; }
    public decimal? payrate { get; set; } = 75;
    public string? clockintime { get; set; } = null;
    public string? clockouttime { get; set; } = null;
}
