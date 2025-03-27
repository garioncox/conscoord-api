using conscoord_api.Data.DTOs;

namespace conscoord_api.Data.Interfaces;

public interface IInvoiceService
{
    Task<List<InvoiceInfoDTO>> GetInvoiceInfoByCompanyTimePeriod(InvoiceDTO DTO);
    Task updateHasBeenInvoiced(employeeInfo rowsEmployee, shiftInfo rowsShift);
    Task<Invoice> CreateInvoice(int CompanyId, DateTime date);
    Task AddURL(int id, string URL);
}
