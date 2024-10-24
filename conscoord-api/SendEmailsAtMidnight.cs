using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Coravel.Invocable;


public class SendEmailsAtMidnight : IInvocable
{
    private IEmployeeShiftService _employeeShiftService;
    private IEmailService _emailController;
    private List<EmployeeShift> _allFutureEmployeeShifts = new List<EmployeeShift>();


    public SendEmailsAtMidnight(IEmployeeShiftService employeeShiftService, IEmailService emailController)
    {
        _employeeShiftService = employeeShiftService;
        _emailController = emailController;
        _allFutureEmployeeShifts = _employeeShiftService.GetFutureShifts();
    }

    public Task Invoke()
    {
        foreach (var employeeShift in _allFutureEmployeeShifts)
        {
            Console.WriteLine("sending email to " + employeeShift.Emp.Email);
            if (employeeShift.Emp.Email != null)
            {
                _emailController.SendEmail(employeeShift.Emp.Email, EmailTemplates.WarnLowOfficerCount.Subject, EmailTemplates.WarnLowOfficerCount.MailBody);
            }
        }
        return Task.CompletedTask;
    }
}
