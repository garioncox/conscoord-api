using System;
using System.Collections.Generic;

namespace conscoord_api.Data;

public partial class Role
{
    public int Id { get; set; }

    public string? Rolename { get; set; }
    public static string PSO_ROLE = "PSO";
    public static string CLIENT_ROLE = "CLIENT";
    public static string ADMIN_ROLE = "ADMIN";

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
