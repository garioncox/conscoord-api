namespace conscoord_api.Data;

public partial class ProjectShift
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int ShiftId { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual Shift Shift { get; set; } = null!;
}
