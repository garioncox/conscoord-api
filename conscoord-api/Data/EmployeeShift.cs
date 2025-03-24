using System;
using System.Collections.Generic;

namespace conscoord_api.Data;

public partial class EmployeeShift
{
    public int Id { get; set; }

    public TimeOnly? ClockInTime { get; set; }

    public TimeOnly? ClockOutTime { get; set; }

    public int EmpId { get; set; }

    public int ShiftId { get; set; }

    public string? Notes { get; set; }

    public bool DidNotWork { get; set; }

    public bool ReportedCanceled { get; set; }

    public bool IsResidual { get; set; }

    public int? InvoiceId { get; set; }

    public virtual Employee Emp { get; set; } = null!;

    public virtual Invoice? Invoice { get; set; }

    public virtual Shift Shift { get; set; } = null!;
}
