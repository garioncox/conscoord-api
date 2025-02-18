using conscoord_api;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace conscoord_tests.Services;

[TestFixture]
public class ShiftClockInReminderTests
{
    [Test]
    public async Task Invoke_SendsEmail_WhenNotClockedIn()
    {
        // Arrange
        var mockShiftService = Substitute.For<IEmployeeShiftService>();
        var mockEmailController = Substitute.For<IEmailService>();

        mockEmailController.SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                           .Returns(new OkResult());

        var shift = new EmployeeShift
        {
            Emp = new Employee { Email = "test@example.com" },
            ClockInTime = null
        };

        mockShiftService.GetShiftsWithinTime(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new List<EmployeeShift> { shift });

        var reminder = new ShiftClockInReminder(mockShiftService, mockEmailController);

        // Act
        await reminder.Invoke();

        // Assert
        mockEmailController.Received(1).SendEmail("test@example.com", EmailTemplates.NotClockedIn.Subject, EmailTemplates.NotClockedIn.MailBody);
    }

    [Test]
    public async Task Invoke_DoesNotSendEmail_WhenClockedIn()
    {
        // Arrange
        var mockShiftService = Substitute.For<IEmployeeShiftService>();
        var mockEmailController = Substitute.For<IEmailService>();

        mockEmailController.SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                       .Returns(new StatusCodeResult(405));

        var shift = new EmployeeShift
        {
            Emp = new Employee { Email = "test@example.com" },
            ClockInTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        };

        mockShiftService.GetShiftsWithinTime(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new List<EmployeeShift> { shift });

        var reminder = new ShiftClockInReminder(mockShiftService, mockEmailController);

        // Act
        await reminder.Invoke();

        // Assert
        mockEmailController.Received(0).SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }
}
