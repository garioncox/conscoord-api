using conscoord_api.Data.DTOs;

namespace conscoord_api.Data.Interfaces;

public interface IInvoiceService
{
    Task<List<InvoiceInfoDTO>> GetInvoiceInfoByCompanyTimePeriod(InvoiceDTO DTO);
}
