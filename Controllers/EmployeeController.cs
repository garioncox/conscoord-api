using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _EmployeeService;
    public EmployeeController(IEmployeeService service)
    {
        _EmployeeService = service;
    }

    [HttpGet("getAll")]
    public async Task<List<Employee>> GetEmployeeListAsync()
    {
        return await _EmployeeService.GetEmployeesListAsync();
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
            Name = employeeDTO.Name
        };
        await _EmployeeService.AddEmployee(employee);
    }
}