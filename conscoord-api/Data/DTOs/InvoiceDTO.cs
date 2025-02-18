namespace conscoord_api.Data.DTOs;

public class InvoiceDTO(int companyId, string startDate, string endDate)
{
    public int companyId { get; set; } = companyId;
    public string startDate { get; set; } = startDate;
    public string endDate { get; set; } = endDate;
}
