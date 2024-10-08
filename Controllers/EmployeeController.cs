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

    [HttpGet("get")]
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

    [HttpGet("GetEmployeeByEmail/{email}")]
    public async Task<Employee> GetEmployeeByEmail(string email)
    {
        return await _EmployeeService.GetEmployeeByEmailAsync(email);
    }

    [HttpPost("add")]
    public async Task PostEmployee([FromBody] EmployeeDTO employeeDTO)
    {
        Employee employee = new Employee()
        {
            Phonenumber = employeeDTO.Phonenumber,
            Email = employeeDTO.Email,
            Name = employeeDTO.Name
        };
        await _EmployeeService.PostEmployee(employee);
    }
}