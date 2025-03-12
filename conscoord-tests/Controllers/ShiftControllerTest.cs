using System.Security.Claims;
using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace conscoord_tests.Controllers;

[TestFixture]
public class ShiftControllerTest
{
    [Test]
    public async Task GetShiftsWithErrors_ReturnsDatesWithShifts_ThatHaveNoClockInTime_ThatHaveNoClockOutTime()
    {
        // ARRANGE
        var mockRoleUtils = Substitute.For<IRoleUtils>();
        mockRoleUtils.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        var mockShiftService = Substitute.For<IShiftService>();
        mockShiftService.GetShiftsWithErrorsByCompany(Arg.Any<int>()).Returns(
            [
                new Shift() { Id = 0, StartTime = DateTime.Parse("2024/12/01 00:00:00"), EndTime = DateTime.Parse("2024/12/01 01:00:00") },
                new Shift() { Id = 1, StartTime = DateTime.Parse("2024/12/02 00:00:00"), EndTime = DateTime.Parse("2024/12/02 01:00:00") },
                new Shift() { Id = 2, StartTime = DateTime.Parse("2024/12/03 00:00:00"), EndTime = DateTime.Parse("2024/12/03 01:00:00") },
                new Shift() { Id = 3, StartTime = DateTime.Parse("2024/12/04 00:00:00"), EndTime = DateTime.Parse("2024/12/04 01:00:00") },
            ]
        );

        var mockEmployeeShiftService = Substitute.For<IEmployeeShiftService>();

        ShiftController controller = new(mockRoleUtils, mockShiftService, mockEmployeeShiftService)
        {
            ControllerContext = TestControllerContext.GetContext(true)
        };

        var companyId = 1;

        // ACT
        var result = await controller.GetDatesWithErrors(companyId);

        // ASSERT
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        
        var okResult = result.Result as OkObjectResult;
        var errorList = okResult?.Value as List<string>;

        Assert.That(errorList, Is.Not.Null);
        Assert.That(errorList, Is.Not.Empty);
        Assert.That(errorList, Has.Count.EqualTo(4));
    }
}