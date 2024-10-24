namespace conscoord_api.Data;

public partial class Role
{
    public int Id { get; set; }

    public string? Rolename { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
