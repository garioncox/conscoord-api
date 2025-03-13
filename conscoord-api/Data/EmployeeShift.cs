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

    public bool HasBeenInvoiced { get; set; } = false;

    public bool DidNotWork { get; set; } = false;

    public bool ReportedCanceled { get; set; } = false;

    public bool IsResidual { get; set; } = false;

    public virtual Employee Emp { get; set; } = null!;

    public virtual Shift Shift { get; set; } = null!;
}
