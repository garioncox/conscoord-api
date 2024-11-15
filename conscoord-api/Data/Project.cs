using System;
using System.Collections.Generic;

namespace conscoord_api.Data;

public partial class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string StartDate { get; set; } = null!;

    public string EndDate { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public int? Contactinfo { get; set; }

    public virtual ICollection<CompanyProject> CompanyProjects { get; set; } = new List<CompanyProject>();

    public virtual Employee? ContactinfoNavigation { get; set; }

    public virtual ICollection<ProjectShift> ProjectShifts { get; set; } = new List<ProjectShift>();
}
