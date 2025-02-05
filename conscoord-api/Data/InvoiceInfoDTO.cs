namespace conscoord_api.Data;

public class InvoiceInfoDTO
{
    public int projectId { get; set; }
    public required string projectName { get; set; }
    public required List<shiftInfo> shiftsByProject { get; set; }
}

public class shiftInfo
{
    public int shiftId { get; set; }
    public required string shiftLocation { get; set; }
    public required List<employeeInfo> employeesByShift { get; set; }
}

public class employeeInfo
{
    public int employeeId { get; set; }
    public required string employeeName { get; set; }
    public decimal employeePayRate { get; set; }
    public double hoursWorked { get; set; }
}
