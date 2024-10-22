using conscoord_api.Controllers;
using conscoord_api.Data.Interfaces;
using Coravel.Invocable;

namespace conscoord_api;

public class ShiftClockInReminder : IInvocable
{
    private readonly IEmployeeShiftService _employeeShiftService;
    private readonly IEmailService _emailController;

    public ShiftClockInReminder(IEmployeeShiftService shiftService, IEmailService emailRequest)
    {
        _employeeShiftService = shiftService;
        _emailController = emailRequest;
    }

    public Task Invoke()
    {
        var end = DateTime.Now;
        var start = DateTime.Now.AddMinutes(-15);
        var shifts = _employeeShiftService.GetShiftsWithinTime(start, end);

        foreach (var shift in shifts)
        {
            if (shift.Emp.Email is not null)
            {
                if (shift.ClockInTime is null)
                {
                    _emailController.SendEmail(shift.Emp.Email, EmailTemplates.NotClockedIn.Subject, EmailTemplates.NotClockedIn.MailBody);
                }
            }
            else
            {
                Console.WriteLine($"{shift.Emp.Email} is associated with the shift at {shift.Shift.Location} : {shift.Shift.Description}");
            }
        }
        return Task.CompletedTask;
    }
}
