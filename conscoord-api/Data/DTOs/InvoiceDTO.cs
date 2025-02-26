namespace conscoord_api.Data.DTOs;

public class InvoiceDTO(int companyId, DateTime startDate, DateTime endDate, bool includeInvoicedShifts)
{
    public bool includeInvoicedShifts { get; set; } = includeInvoicedShifts;
    public int companyId { get; set; } = companyId;
    public DateTime startDate { get; set; } = startDate;
    public DateTime endDate { get; set; } = endDate;
}
