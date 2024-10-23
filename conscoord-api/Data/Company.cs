namespace conscoord_api.Data;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<CompanyProject> CompanyProjects { get; set; } = new List<CompanyProject>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}