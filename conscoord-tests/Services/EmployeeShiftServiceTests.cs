using System.Security.Claims;
using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using conscoord_tests.Controllers;
using NSubstitute;

namespace conscoord_tests.Services;

internal class EmployeeShiftServiceTests
{
    [Test]
    public async Task TestSigningUpForShift_ShouldNotAllowOverlappingShifts()
    {
        // ARRANGE
        List<Shift> shifts =
        [
            new Shift()
            {
                Id = 0,
                StartTime = DateTime.MinValue.ToString(),
                EndTime = DateTime.MinValue.AddHours(8).ToString(),
                RequestedEmployees = 10
            },
            new Shift()
            {
                Id = 1,
                StartTime = DateTime.MinValue.AddHours(4).ToString(),
                EndTime = DateTime.MinValue.AddHours(12).ToString(),
                RequestedEmployees = 10
            },
        ];

        EmployeeShiftDTO DTO = new()
        {
            EmpId = 1,
            ShiftId = 0
        };

        var shiftServiceMock = Substitute.For<IShiftService>();
        shiftServiceMock
            .GetShiftById(Arg.Any<int>())
            .Returns(shifts[0]);
        shiftServiceMock
            .GetScheduledShiftsByEmpId(Arg.Any<int>())
            .Returns([shifts[1]]);

        var empShiftServiceMock = Substitute.For<IEmployeeShiftService>();

        var roleUtilsMock = Substitute.For<IRoleUtils>();
        roleUtilsMock.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        EmployeeShiftController controller = new(empShiftServiceMock, shiftServiceMock, roleUtilsMock)
        {
            ControllerContext = TestControllerContext.GetContext()
        };

        // ACT
        await controller.CreateEmpShift(DTO);

        // ASSERT
        await shiftServiceMock.Received(1).GetShiftById(Arg.Any<int>());
        shiftServiceMock.Received(1).GetScheduledShiftsByEmpId(Arg.Any<int>());
        await empShiftServiceMock.Received(0).CreateEmployeeShift(Arg.Any<EmployeeShift>());
    }

    [Test]
    public async Task TestSigningUpForShift_CannotSignUpForSameShift()
    {
        // ARRANGE
        List<Shift> shifts =
        [
            new Shift()
            {
                Id = 0,
                StartTime = DateTime.MinValue.ToString(),
                EndTime = DateTime.MinValue.AddHours(8).ToString(),
                RequestedEmployees = 10
            },
        ];

        EmployeeShiftDTO DTO = new()
        {
            EmpId = 1,
            ShiftId = 0
        };

        var shiftServiceMock = Substitute.For<IShiftService>();
        shiftServiceMock
            .GetShiftById(Arg.Any<int>())
            .Returns(shifts[0]);
        shiftServiceMock
            .GetScheduledShiftsByEmpId(Arg.Any<int>())
            .Returns([shifts[0]]);

        var empShiftServiceMock = Substitute.For<IEmployeeShiftService>();

        var roleUtilsMock = Substitute.For<IRoleUtils>();
        roleUtilsMock.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        EmployeeShiftController controller = new(empShiftServiceMock, shiftServiceMock, roleUtilsMock)
        {
            ControllerContext = TestControllerContext.GetContext()
        };

        // ACT
        await controller.CreateEmpShift(DTO);

        // ASSERT
        await shiftServiceMock.Received(1).GetShiftById(Arg.Any<int>());
        shiftServiceMock.Received(1).GetScheduledShiftsByEmpId(Arg.Any<int>());
        await empShiftServiceMock.Received(0).CreateEmployeeShift(Arg.Any<EmployeeShift>());
    }
}
