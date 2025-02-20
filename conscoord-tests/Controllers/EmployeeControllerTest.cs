using System.Security.Claims;
using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using conscoord_api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace conscoord_tests.Controllers;

[TestFixture]
public class EmployeeControllerTests
{
    [Test]
    public async Task WhenUserNotAuthenticated_ReturnsNotFound()
    {
        // ARRANGE
        var mockEmployeeService = Substitute.For<IEmployeeService>();
        var mockRoleUtils = Substitute.For<IRoleUtils>();
        mockRoleUtils.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        var controller = new EmployeeController(mockRoleUtils, mockEmployeeService)
        {
            ControllerContext = TestControllerContext.GetContext(false)
        };

        // ACT
        var actual = await controller.GetCurrentUser();

        // Assert
        Assert.That(actual.Result, Is.InstanceOf<NotFoundResult>());

        await mockEmployeeService.Received(0).GetEmployeeByEmailAsync(Arg.Any<string>());
        await mockEmployeeService.Received(0).AddEmployee(Arg.Any<Employee>());
    }

    [Test]
    public async Task WhenUserExists_GetCurrentUser_ReturnsThatUser()
    {
        // ARRANGE
        Employee mockEmployee = new() { Id = 1, Email = "test@demo.com" };

        var mockEmployeeService = Substitute.For<IEmployeeService>();
        var mockRoleUtils = Substitute.For<IRoleUtils>();
        mockRoleUtils.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        var controller = new EmployeeController(mockRoleUtils, mockEmployeeService);
        var context = Substitute.For<HttpContext>();
        var user = Substitute.For<ClaimsPrincipal>();
        var identity = Substitute.For<ClaimsIdentity>();

        mockEmployeeService.GetEmployeeByEmailAsync(mockEmployee.Email)
            .Returns(mockEmployee);

        identity.IsAuthenticated.Returns(true);

        user.Identity.Returns(identity);
        user.FindFirst(ClaimTypes.Email).Returns(new Claim(ClaimTypes.Email, mockEmployee.Email));

        context.User.Returns(user);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };

        // ACT
        var actual = await controller.GetCurrentUser();

        // Assert
        Assert.That(actual.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = actual.Result as OkObjectResult;

        Assert.That(okResult?.Value, Is.InstanceOf<Employee>());
        Assert.That(((Employee)okResult?.Value)?.Id, Is.EqualTo(mockEmployee.Id));

        await mockEmployeeService.Received(1).GetEmployeeByEmailAsync(mockEmployee.Email);
        await mockEmployeeService.Received(0).AddEmployee(Arg.Any<Employee>());
    }

    [Fact]
    public async Task WhenUserDoesNotExist_GetCurrentUser_CreatesUser()
    {
        // ARRANGE
        Employee mockEmployee = new() { Id = 1, Email = "test@demo.com", Name = "john doe" };

        var mockEmployeeService = Substitute.For<IEmployeeService>();
        var mockRoleUtils = Substitute.For<RoleUtils>();
        mockRoleUtils.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        var controller = new EmployeeController(mockRoleUtils, mockEmployeeService);
        var context = Substitute.For<HttpContext>();
        var user = Substitute.For<ClaimsPrincipal>();
        var identity = Substitute.For<ClaimsIdentity>();

        mockEmployeeService.GetEmployeeByEmailAsync(mockEmployee.Email)
            .Returns(Task.FromResult<Employee?>(null));

        identity.IsAuthenticated.Returns(true);

        user.Identity.Returns(identity);
        user.FindFirst(ClaimTypes.Email).Returns(new Claim(ClaimTypes.Email, mockEmployee.Email));
        user.FindFirst(ClaimTypes.Name).Returns(new Claim(ClaimTypes.Email, mockEmployee.Name));

        context.User.Returns(user);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };

        // ACT
        var actual = await controller.GetCurrentUser();

        // Assert
        Assert.That(actual.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = actual.Result as OkObjectResult;

        Assert.That(okResult?.Value, Is.InstanceOf<Employee>());
        Assert.That(((Employee)okResult?.Value)?.Id, Is.EqualTo(mockEmployee.Id));

        await mockEmployeeService.Received(2).GetEmployeeByEmailAsync(mockEmployee.Email);
        await mockEmployeeService.Received(1).AddEmployee(Arg.Any<Employee>());
    }
}
