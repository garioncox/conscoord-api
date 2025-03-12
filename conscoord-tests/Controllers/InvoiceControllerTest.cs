using System.Security.Claims;
using conscoord_api.Controllers;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace conscoord_tests.Controllers;

[TestFixture]
public class InvoiceControllerTests
{
    [Test]
    public async Task GetInvoicePreview_WithStartDateBeforeEndDate_ReturnsList()
    {
        // ARRANGE
        var mockInvoiceService = Substitute.For<IInvoiceService>();
        var mockRoleService = Substitute.For<IRoleUtils>();
        mockRoleService.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        InvoiceController controller = new(mockInvoiceService, mockRoleService)
        {
            ControllerContext = TestControllerContext.GetContext()
        };

        var companyId = 0;
        var startDate = DateTime.Parse("2025/02/18");
        var endDate = DateTime.Parse("2025/02/19");
        InvoiceDTO dto = new(companyId, startDate, endDate, false);

        // ACT
        await controller.GetInvoiceInfoByCompanyTimePeriod(dto);

        // ASSERT
        await mockInvoiceService.Received(1).GetInvoiceInfoByCompanyTimePeriod(Arg.Any<InvoiceDTO>());
    }

    [Test]
    public async Task GetInvoicePreview_WithStartDateDuringEndDate_ReturnsList()
    {
        // ARRANGE
        var mockInvoiceService = Substitute.For<IInvoiceService>();
        var mockRoleService = Substitute.For<IRoleUtils>();
        mockRoleService.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        InvoiceController controller = new(mockInvoiceService, mockRoleService)
        {
            ControllerContext = TestControllerContext.GetContext()
        };

        var companyId = 0;
        var startDate = DateTime.Parse("2025/02/18");
        var endDate = startDate;
        InvoiceDTO dto = new(companyId, startDate, endDate, false);

        // ACT
        await controller.GetInvoiceInfoByCompanyTimePeriod(dto);

        // ASSERT
        await mockInvoiceService.Received(1).GetInvoiceInfoByCompanyTimePeriod(Arg.Any<InvoiceDTO>());
    }

    [Test]
    public async Task GetInvoicePreview_WithStartDateAfterEndDate_ReturnsBadRequest()
    {
        // ARRANGE
        var mockInvoiceService = Substitute.For<IInvoiceService>();
        var mockRoleService = Substitute.For<IRoleUtils>();
        mockRoleService.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        InvoiceController controller = new(mockInvoiceService, mockRoleService)
        {
            ControllerContext = TestControllerContext.GetContext()
        };

        var companyId = 0;
        var startDate = DateTime.Parse("2025/02/19");
        var endDate = DateTime.Parse("2025/02/18");
        InvoiceDTO dto = new(companyId, startDate, endDate, false);

        // ACT
        var result = await controller.GetInvoiceInfoByCompanyTimePeriod(dto);
        var badRequestResult = result.Result as BadRequestObjectResult;

        // ASSERT
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.EqualTo("Please make sure the Start Date is before the End Date"));
    }
}
