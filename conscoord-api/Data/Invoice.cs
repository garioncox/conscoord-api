using System;
using System.Collections.Generic;

namespace conscoord_api.Data;

public partial class Invoice
{
    public int Id { get; set; }

    public Guid InvoiceNumber { get; set; }

    public string? InvoiceUrl { get; set; }

    public int CompanyId { get; set; }

    public DateTime PostedDate { get; set; }

    public virtual Company? Company { get; set; }

    public virtual ICollection<EmployeeShift> EmployeeShifts { get; set; } = new List<EmployeeShift>();
}
