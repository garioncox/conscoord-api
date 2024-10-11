using conscoord_api.Controllers;
using conscoord_api.Data.Interfaces;
using Coravel.Invocable;

namespace conscoord_api
{
  public class ShiftClockInReminder : IInvocable
  {
    IEmployeeShiftService _employeeShiftService;
    EmailController _emailRequest;
     public ShiftClockInReminder(IEmployeeShiftService shiftService, EmailController emailRequest)
    {
      _employeeShiftService = shiftService;
      _emailRequest = emailRequest;
    }
    public Task Invoke()
    {
      var end = DateTime.Now;
      var start = DateTime.Now.AddMinutes(-15) ;
      var shifts = _employeeShiftService.GetShiftsWithinTime(start, end);

      foreach (var shift in shifts)
      {
        if (shift.Emp.Email is not null)
        { 
          if (shift.ClockInTime is null)
            _emailRequest.SendEmailBackend(shift.Emp.Email, EmailTemplates.NotClockedIn.Subject, EmailTemplates.NotClockedIn.MailBody);
        }
        else
          Console.WriteLine("No employee associated with the shift");
      }
      return Task.CompletedTask;
    }
  }
}
