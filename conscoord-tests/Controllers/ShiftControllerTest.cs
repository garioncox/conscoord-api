using System.Security.Claims;
using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
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
        mockShiftService.GetAllShifts().Returns(
            [
                new Shift() { Id = 0, StartTime = "2024/12/01 00:00:00", EndTime = "2024/12/01 01:00:00" },
                new Shift() { Id = 1, StartTime = "2024/12/02 00:00:00", EndTime = "2024/12/02 01:00:00" },
                new Shift() { Id = 2, StartTime = "2024/12/03 00:00:00", EndTime = "2024/12/03 01:00:00" },
                new Shift() { Id = 3, StartTime = "2024/12/04 00:00:00", EndTime = "2024/12/04 01:00:00" },
            ]
        );

        var mockEmployeeShiftService = Substitute.For<IEmployeeShiftService>();
        mockEmployeeShiftService.GetallEmployeeShifts().Returns([
            new EmployeeShift() { Id = 0, ShiftId = 0, ClockInTime = "2024/12/01 00:00:00", ClockOutTime = "2024/12/01 01:00:00" },
            new EmployeeShift() { Id = 1, ShiftId = 1, ClockInTime = "", ClockOutTime = "" },
            new EmployeeShift() { Id = 2, ShiftId = 2, ClockOutTime = "2024/12/03 01:00:00" },
            new EmployeeShift() { Id = 3, ShiftId = 3 },
        ]);

        ShiftController controller = new(mockRoleUtils, mockShiftService, mockEmployeeShiftService)
        {
            ControllerContext = TestControllerContext.GetContext(true)
        };

        // ACT
        var errors = await controller.GetDatesWithErrors();

        // ASSERT
        Assert.That(errors, Is.Not.Empty);
        Assert.That(errors, Has.Count.EqualTo(3));
        Assert.That(errors, Does.Contain("2024/12/02"));
        Assert.That(errors, Does.Contain("2024/12/03"));
        Assert.That(errors, Does.Contain("2024/12/04"));
    }
}