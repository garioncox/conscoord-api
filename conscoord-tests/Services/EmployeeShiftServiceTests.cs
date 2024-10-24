using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Moq;

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
            EmployeeId = 1,
            ShiftId = 0
        };

        var shiftServiceMock = new Mock<IShiftService>();
        shiftServiceMock.Setup(m => m
            .GetShiftById(It.IsAny<int>()))
            .ReturnsAsync(shifts[0]);

        var empShiftServiceMock = new Mock<IEmployeeShiftService>();
        empShiftServiceMock.Setup(m => m
            .GetScheduledShiftsByEmpId(It.IsAny<int>()))
            .Returns([shifts[1]]);

        // ACT
        EmployeeShiftController empShiftController = new(empShiftServiceMock.Object, shiftServiceMock.Object);
        await empShiftController.CreateEmpShift(DTO);

        // ASSERT
        shiftServiceMock.Verify(m => m.GetShiftById(It.IsAny<int>()), Times.Once());
        empShiftServiceMock.Verify(m => m.GetScheduledShiftsByEmpId(It.IsAny<int>()), Times.Once());
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
            EmployeeId = 1,
            ShiftId = 0
        };

        var shiftServiceMock = new Mock<IShiftService>();
        shiftServiceMock.Setup(m => m
            .GetShiftById(It.IsAny<int>()))
            .ReturnsAsync(shifts[0]);

        var empShiftServiceMock = new Mock<IEmployeeShiftService>();
        empShiftServiceMock.Setup(m => m
            .GetScheduledShiftsByEmpId(It.IsAny<int>()))
            .Returns([shifts[0]]);

        // ACT
        EmployeeShiftController empShiftController = new(empShiftServiceMock.Object, shiftServiceMock.Object);
        await empShiftController.CreateEmpShift(DTO);

        // ASSERT
        shiftServiceMock.Verify(m => m.GetShiftById(It.IsAny<int>()), Times.Once());
        empShiftServiceMock.Verify(m => m.GetScheduledShiftsByEmpId(It.IsAny<int>()), Times.Once());
        empShiftServiceMock.Verify(m => m.CreateEmployeeShift(It.IsAny<EmployeeShift>()), Times.Never());
    }
}
