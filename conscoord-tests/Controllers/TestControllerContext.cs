using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace conscoord_tests.Controllers;
public static class TestControllerContext
{
    public static ControllerContext GetContext(bool IsAuthenticated = true)
    {
        var context = Substitute.For<HttpContext>();
        var user = Substitute.For<ClaimsPrincipal>();
        var identity = Substitute.For<ClaimsIdentity>();

        identity.IsAuthenticated.Returns(IsAuthenticated);
        user.Identity.Returns(identity);
        context.User.Returns(user);
        return new ControllerContext
        {
            HttpContext = context
        };
    }
}