using conscoord_api;
using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace conscoord_tests.Services;

[TestFixture]
public class ShiftClockInReminderTests
{
    [Test]
    public async Task Invoke_SendsEmail_WhenNotClockedIn()
    {
        // Arrange
        var mockShiftService = new Mock<IEmployeeShiftService>();
        var mockEmailController = new Mock<IEmailService>() { CallBase = true };

        mockEmailController.Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(new OkResult());

        var shift = new EmployeeShift
        {
            Emp = new Employee { Email = "test@example.com" },
            ClockInTime = null
        };

        mockShiftService.Setup(s => s.GetShiftsWithinTime(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(new List<EmployeeShift> { shift });

        var reminder = new ShiftClockInReminder(mockShiftService.Object, mockEmailController.Object);

        // Act
        await reminder.Invoke();

        // Assert
        mockEmailController.Verify(e => e.SendEmail("test@example.com", EmailTemplates.NotClockedIn.Subject, EmailTemplates.NotClockedIn.MailBody), Times.Once);
    }

    [Test]
    public async Task Invoke_DoesNotSendEmail_WhenClockedIn()
    {
        // Arrange
        var mockShiftService = new Mock<IEmployeeShiftService>();
        var mockEmailController = new Mock<IEmailService>() { CallBase = true };

        mockEmailController.Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                       .Returns(new StatusCodeResult(405));

        var shift = new EmployeeShift
        {
            Emp = new Employee { Email = "test@example.com" },
            ClockInTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        };

        mockShiftService.Setup(s => s.GetShiftsWithinTime(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(new List<EmployeeShift> { shift });

        var reminder = new ShiftClockInReminder(mockShiftService.Object, mockEmailController.Object);

        // Act
        await reminder.Invoke();

        // Assert
        mockEmailController.Verify(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
