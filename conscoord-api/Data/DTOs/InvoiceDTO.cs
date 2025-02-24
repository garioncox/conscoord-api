namespace conscoord_api.Data.DTOs;

public class InvoiceDTO(int companyId, string startDate, string endDate, bool includeInvoicedShifts)
{
    public bool includeInvoicedShifts { get; set; } = includeInvoicedShifts;
    public bool includeErroredShifts { get; set; } = includeInvoicedShifts;
    public int companyId { get; set; } = companyId;
    public string startDate { get; set; } = startDate;
    public string endDate { get; set; } = endDate;
}
