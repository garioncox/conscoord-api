using System.Security.Claims;
using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using conscoord_api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NSubstitute;

namespace conscoord_tests.Services;

internal class EmployeeShiftServiceTests
{
    [SetUp]
    public void Setup()
    {
    }

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

        var shiftServiceMock = new Mock<IShiftService>();
        shiftServiceMock.Setup(m => m
            .GetShiftById(It.IsAny<int>()))
            .ReturnsAsync(shifts[0]);
        shiftServiceMock.Setup(m => m
            .GetScheduledShiftsByEmpId(It.IsAny<int>()))
            .Returns([shifts[1]]);

        var empShiftServiceMock = new Mock<IEmployeeShiftService>();

        var roleUtilsMock = Substitute.For<IRoleUtils>();
        roleUtilsMock.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        EmployeeShiftController controller = new(empShiftServiceMock.Object, shiftServiceMock.Object, roleUtilsMock);

        var context = Substitute.For<HttpContext>();
        var user = Substitute.For<ClaimsPrincipal>();
        var identity = Substitute.For<ClaimsIdentity>();

        identity.IsAuthenticated.Returns(true);

        user.Identity.Returns(identity);

        context.User.Returns(user);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };

        // ACT
        await controller.CreateEmpShift(DTO);

        // ASSERT
        shiftServiceMock.Verify(m => m.GetShiftById(It.IsAny<int>()), Times.Once());
        shiftServiceMock.Verify(m => m.GetScheduledShiftsByEmpId(It.IsAny<int>()), Times.Once());
        empShiftServiceMock.Verify(m => m.CreateEmployeeShift(It.IsAny<EmployeeShift>()), Times.Never());
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

        var shiftServiceMock = new Mock<IShiftService>();
        shiftServiceMock.Setup(m => m
            .GetShiftById(It.IsAny<int>()))
            .ReturnsAsync(shifts[0]);
        shiftServiceMock.Setup(m => m
            .GetScheduledShiftsByEmpId(It.IsAny<int>()))
            .Returns([shifts[0]]);

        var empShiftServiceMock = new Mock<IEmployeeShiftService>();

        var roleUtilsMock = Substitute.For<IRoleUtils>();
        roleUtilsMock.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        var context = Substitute.For<HttpContext>();
        var user = Substitute.For<ClaimsPrincipal>();
        var identity = Substitute.For<ClaimsIdentity>();

        EmployeeShiftController controller = new(empShiftServiceMock.Object, shiftServiceMock.Object, roleUtilsMock);

        identity.IsAuthenticated.Returns(true);
        user.Identity.Returns(identity);
        context.User.Returns(user);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };

        // ACT
        await controller.CreateEmpShift(DTO);

        // ASSERT
        shiftServiceMock.Verify(m => m.GetShiftById(It.IsAny<int>()), Times.Once());
        shiftServiceMock.Verify(m => m.GetScheduledShiftsByEmpId(It.IsAny<int>()), Times.Once());
        empShiftServiceMock.Verify(m => m.CreateEmployeeShift(It.IsAny<EmployeeShift>()), Times.Never());
    }
}
