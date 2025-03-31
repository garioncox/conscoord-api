using conscoord_api.Data.DTOs;

namespace conscoord_api.Data.Interfaces;

public interface IInvoiceService
{
    Task<List<InvoiceInfoDTO>> GetInvoiceInfoByCompanyTimePeriod(InvoiceDTO DTO);

    Task updateHasBeenInvoiced(employeeInfo rowsEmployee, shiftInfo rowsShift, int invoiceId);
    Task<Invoice> CreateInvoice(int CompanyId, string Url);
    Task<List<AzureInvoiceDTO>> GetAllInvoicesByCompany(int companyId);

}
