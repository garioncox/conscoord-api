namespace conscoord_api.Data;

public partial class EmployeeShift
{
    public int Id { get; set; }

    public string? ClockInTime { get; set; }

    public string? ClockOutTime { get; set; }

    public int EmpId { get; set; }

    public int ShiftId { get; set; }

    public virtual Employee Emp { get; set; } = null!;

    public virtual Shift Shift { get; set; } = null!;
}
