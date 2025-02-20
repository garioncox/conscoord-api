using System.Security.Claims;
using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using conscoord_api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _EmployeeService;
    private readonly IRoleUtils _roleUtils;

    public EmployeeController(IRoleUtils roleservice, IEmployeeService service)
    {
        _EmployeeService = service;
        _roleUtils = roleservice;
    }

    [HttpGet("getAll")]
    public async Task<List<Employee>> GetEmployeeListAsync()
    {
        return await _EmployeeService.GetEmployeesListAsync();
    }

    [HttpGet("getAll/{shift_id}")]
    public async Task<List<Employee>> GetEmployeesByShiftId(int shift_id)
    {
        var user = HttpContext.User;
        var hasPerms = await _roleUtils.HasPerms(user, [Role.ADMIN_ROLE, Role.CLIENT_ROLE]);
        if (!hasPerms) { return []; }

        Console.WriteLine("received request with shiftID: " + shift_id);
        return await _EmployeeService.GetEmployeesByShiftIdAsync(shift_id);
    }

    [HttpGet("getCurrentUser")]
    public async Task<ActionResult<Employee>> GetCurrentUser()
    {
        var user = HttpContext.User;
        if (user.Identity?.IsAuthenticated == false) { return NotFound(); }

        var userEmail = user?.FindFirst(ClaimTypes.Email)?.Value;
        var employee = await _EmployeeService.GetEmployeeByEmailAsync(userEmail ?? "");

        // Create employee if not in DB
        if (employee == null)
        {
            EmployeeDTO dto = new()
            {
                Name = user?.FindFirst(ClaimTypes.Name)?.Value ?? "",
                Email = user?.FindFirst(ClaimTypes.Email)?.Value ?? "",
                Phonenumber = ""
            };
            await AddEmployee(dto);
            employee = await _EmployeeService.GetEmployeeByEmailAsync(userEmail ?? "");
        }

        return Ok(employee);
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(int id)
    {
        var employee = await _EmployeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpGet("getByEmail/{email}")]
    public async Task<ActionResult<Employee>> GetEmployeeByEmail(string email)
    {
        var employee = await _EmployeeService.GetEmployeeByEmailAsync(email); ;
        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpPost("add")]
    public async Task AddEmployee([FromBody] EmployeeDTO employeeDTO)
    {
        Employee employee = new Employee()
        {
            Phonenumber = employeeDTO.Phonenumber,
            Email = employeeDTO.Email,
            Name = employeeDTO.Name,
            Companyid = employeeDTO.Companyid,
        };
        await _EmployeeService.AddEmployee(employee);
    }

    [HttpPut("edit")]
    public async Task EditEmployee([FromBody] Employee employee)
    {
        await _EmployeeService.EditEmployee(employee);
    }

    [HttpGet("GetByShift/{shiftId}")]
    public async Task<ActionResult<List<Employee>>> GetEmployeeSignedUpForShift(int shiftId)
    {
        var response = await _EmployeeService.GetEmployeesSignedUpForShift(shiftId);
        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }
}
