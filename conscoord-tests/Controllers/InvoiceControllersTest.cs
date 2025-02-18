using conscoord_api.Controllers;
using conscoord_api.Data.Interfaces;
using NSubstitute;

namespace conscoord_tests.Controllers;

[TestFixture]
public class InvoiceControllerTests
{
    [Test]
    public void GetInvoicePreview_WithStartDateAfterEndDate_ReturnsBadRequest()
    {
        var mockInvoiceService = Substitute.For<IInvoiceService>();
        var mockRoleService = Substitute.For<IRoleUtils>();
        InvoiceController controller = new(mockInvoiceService, mockRoleService);
    }
}