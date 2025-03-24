using System.Security.Claims;
using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace conscoord_tests.Controllers;

[TestFixture]
public class ProjectShiftControllerTest
{
    [Test]
    public async Task CreateProjectShift_WithProjectIdNotInDb_ReturnsBadRequest()
    {
        // ARRANGE
        var projectShiftServiceMock = Substitute.For<IProjectShiftService>();
        var projectServiceMock = Substitute.For<IProjectService>();
        var shiftServiceMock = Substitute.For<IShiftService>();
        var roleUtilsMock = Substitute.For<IRoleUtils>();
        roleUtilsMock.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        ProjectShiftController controller = new(projectShiftServiceMock, projectServiceMock, shiftServiceMock, roleUtilsMock)
        {
            ControllerContext = TestControllerContext.GetContext()
        };

        ProjectShiftDTO dto = new()
        {
            ProjectId = 0,
            Shift = new()
            {
                StartTime = DateTime.Parse("2024/01/01"),
                EndTime = DateTime.Parse("2024/01/31"),
                Status = Shift.STATUS_ACTIVE
            }
        };

        // ACT
        var actual = await controller.CreateProjectShift(dto);

        // ASSERT
        Assert.That(actual, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task CreateProjectShift_WithShiftTimesBeforeProjectTimeline_ReturnsBadRequest()
    {
        // ARRANGE
        var projectShiftServiceMock = Substitute.For<IProjectShiftService>();
        var projectServiceMock = Substitute.For<IProjectService>();
        projectServiceMock.GetProjectByIdAsync(Arg.Any<int>())
            .Returns(new Project()
            {
                StartDate = DateTime.Parse("2025/12/01"),
                EndDate = DateTime.Parse("2025/12/31")
            });
        var shiftServiceMock = Substitute.For<IShiftService>();
        var roleUtilsMock = Substitute.For<IRoleUtils>();
        roleUtilsMock.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        ProjectShiftController controller = new(projectShiftServiceMock, projectServiceMock, shiftServiceMock, roleUtilsMock)
        {
            ControllerContext = TestControllerContext.GetContext()
        };

        ProjectShiftDTO dto = new()
        {
            ProjectId = 0,
            Shift = new()
            {
                StartTime = DateTime.Parse("2024/01/01"),
                EndTime = DateTime.Parse("2024/01/31"),
                Status = Shift.STATUS_ACTIVE
            }
        };

        // ACT
        var actual = await controller.CreateProjectShift(dto);

        // ASSERT
        Assert.That(actual, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task CreateProjectShift_WithShiftTimesAfterProjectTimeline_ReturnsBadRequest()
    {
        // ARRANGE
        var projectShiftServiceMock = Substitute.For<IProjectShiftService>();
        var projectServiceMock = Substitute.For<IProjectService>();
        projectServiceMock.GetProjectByIdAsync(Arg.Any<int>())
            .Returns(new Project()
            {
                StartDate = DateTime.Parse("2024/12/01"),
                EndDate = DateTime.Parse("2024/12/31")
            });
        var shiftServiceMock = Substitute.For<IShiftService>();
        var roleUtilsMock = Substitute.For<IRoleUtils>();
        roleUtilsMock.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        ProjectShiftController controller = new(projectShiftServiceMock, projectServiceMock, shiftServiceMock, roleUtilsMock)
        {
            ControllerContext = TestControllerContext.GetContext()
        };

        ProjectShiftDTO dto = new()
        {
            ProjectId = 0,
            Shift = new()
            {
                StartTime = DateTime.Parse("2025/01/01"),
                EndTime = DateTime.Parse("2025/01/31"),
                Status = Shift.STATUS_ACTIVE
            }
        };

        // ACT
        var actual = await controller.CreateProjectShift(dto);

        // ASSERT
        Assert.That(actual, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task CreateProjectShift_WithShiftTimesInProjectTimeline_ReturnsOk()
    {
        // ARRANGE
        var projectShiftServiceMock = Substitute.For<IProjectShiftService>();
        var projectServiceMock = Substitute.For<IProjectService>();
        projectServiceMock.GetProjectByIdAsync(Arg.Any<int>())
            .Returns(new Project()
            {
                StartDate = DateTime.Parse("2024/12/01"),
                EndDate = DateTime.Parse("2024/12/31")
            });
        var shiftServiceMock = Substitute.For<IShiftService>();
        var roleUtilsMock = Substitute.For<IRoleUtils>();
        roleUtilsMock.HasPerms(Arg.Any<ClaimsPrincipal>(), Arg.Any<string[]>()).Returns(true);

        ProjectShiftController controller = new(projectShiftServiceMock, projectServiceMock, shiftServiceMock, roleUtilsMock)
        {
            ControllerContext = TestControllerContext.GetContext()
        };

        ProjectShiftDTO dto = new()
        {
            ProjectId = 0,
            Shift = new()
            {
                StartTime = DateTime.Parse("2024/12/10"),
                EndTime = DateTime.Parse("2024/12/10"),
                Status = Shift.STATUS_ACTIVE
            }
        };

        // ACT
        var actual = await controller.CreateProjectShift(dto);

        // ASSERT
        Assert.That(actual, Is.InstanceOf<OkResult>());
    }
}
