namespace conscoord_api.Data;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phonenumber { get; set; }

    public int? Roleid { get; set; }

    public int? Companyid { get; set; }

    public virtual Company? Company { get; set; }

    public virtual ICollection<EmployeeShift> EmployeeShifts { get; set; } = new List<EmployeeShift>();

    public virtual Role? Role { get; set; }
}
