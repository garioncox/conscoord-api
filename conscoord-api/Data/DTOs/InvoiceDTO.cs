namespace conscoord_api.Data.DTOs;

public class InvoiceDTO
{
    public int companyId { get; set; }
    public required string startDate { get; set; }
    public required string endDate { get; set; }
    public required bool includeInvoicedShifts { get; set; }
}
