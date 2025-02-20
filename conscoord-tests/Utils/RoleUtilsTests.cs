using System.Security.Claims;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using conscoord_api.Utils;
using NSubstitute;

namespace conscoord_tests.Utils;

[TestFixture]
public class RoleUtilsTests
{
    [Test]
    public async Task NullUser_HasNoPerms()
    {
        // ARRANGE
        var mockRoleService = Substitute.For<IRoleService>();
        RoleUtils utils = new(mockRoleService);

        // ACT
        var result = await utils.HasPerms(null, []);

        // ASSERT
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task NonAuthenticatedUser_HasNoPerms()
    {
        // ARRANGE
        var mockRoleService = Substitute.For<IRoleService>();

        var mockUser = Substitute.For<ClaimsPrincipal>();
        var mockIdentity = Substitute.For<ClaimsIdentity>();
        mockUser.Identity.Returns(mockIdentity);
        mockIdentity.IsAuthenticated.Returns(false);

        RoleUtils utils = new(mockRoleService);

        // ACT
        var result = await utils.HasPerms(mockUser, []);

        // ASSERT
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task AuthenticatedUser_WithoutRole_HasNoPerms()
    {
        // ARRANGE
        var mockRoleService = Substitute.For<IRoleService>();
        mockRoleService.GetRoleByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult((Role?)null));

        var mockUser = Substitute.For<ClaimsPrincipal>();
        var mockIdentity = Substitute.For<ClaimsIdentity>();
        mockUser.Identity.Returns(mockIdentity);
        mockIdentity.IsAuthenticated.Returns(true);
        mockUser.FindFirst(ClaimTypes.Email)
            .Returns(new Claim(ClaimTypes.Email, "jdoe@test.com"));

        RoleUtils utils = new(mockRoleService);

        // ACT
        var result = await utils.HasPerms(mockUser, []);

        // ASSERT
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task AuthenticatedUser_WithRoleInDb_WithNoRoleSpecified_HasPerms()
    {
        // ARRANGE
        Role mockRole = new() { Id = 0, Rolename = Role.PSO_ROLE };

        var mockRoleService = Substitute.For<IRoleService>();
        mockRoleService.GetRoleByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult((Role?)mockRole));

        var mockUser = Substitute.For<ClaimsPrincipal>();
        var mockIdentity = Substitute.For<ClaimsIdentity>();
        mockUser.Identity.Returns(mockIdentity);
        mockIdentity.IsAuthenticated.Returns(true);
        mockUser.FindFirst(ClaimTypes.Email)
            .Returns(new Claim(ClaimTypes.Email, "jdoe@test.com"));

        RoleUtils utils = new(mockRoleService);

        // ACT
        var result = await utils.HasPerms(mockUser, []);

        // ASSERT
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task AuthenticatedUser_WithClientRoleInDb_WithClientAndAdminRoleSpecified_HasPerms()
    {
        // ARRANGE
        Role mockRole = new() { Id = 0, Rolename = Role.CLIENT_ROLE };

        var mockRoleService = Substitute.For<IRoleService>();
        mockRoleService.GetRoleByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult((Role?)mockRole));

        var mockUser = Substitute.For<ClaimsPrincipal>();
        var mockIdentity = Substitute.For<ClaimsIdentity>();
        mockUser.Identity.Returns(mockIdentity);
        mockIdentity.IsAuthenticated.Returns(true);
        mockUser.FindFirst(ClaimTypes.Email)
            .Returns(new Claim(ClaimTypes.Email, "jdoe@test.com"));

        RoleUtils utils = new(mockRoleService);

        // ACT
        var result = await utils.HasPerms(mockUser, [Role.CLIENT_ROLE, Role.ADMIN_ROLE]);

        // ASSERT
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task AuthenticatedUser_WithClientRoleInDb_WithPSORoleSpecified_HasNoPerms()
    {
        // ARRANGE
        Role mockRole = new() { Id = 0, Rolename = Role.CLIENT_ROLE };

        var mockRoleService = Substitute.For<IRoleService>();
        mockRoleService.GetRoleByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult((Role?)mockRole));

        var mockUser = Substitute.For<ClaimsPrincipal>();
        var mockIdentity = Substitute.For<ClaimsIdentity>();
        mockUser.Identity.Returns(mockIdentity);
        mockIdentity.IsAuthenticated.Returns(true);
        mockUser.FindFirst(ClaimTypes.Email)
            .Returns(new Claim(ClaimTypes.Email, "jdoe@test.com"));

        RoleUtils utils = new(mockRoleService);

        // ACT
        var result = await utils.HasPerms(mockUser, [Role.PSO_ROLE]);

        // ASSERT
        Assert.That(result, Is.False);
    }
}
