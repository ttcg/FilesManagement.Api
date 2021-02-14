using FilesManagement.Api.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace FilesManagement.UnitTests
{
    public class WhenUsingAllowedUserAttribute
    {

        private MockController controller;
        private AuthorizationFilterContext authorizationFilterContext;

        public WhenUsingAllowedUserAttribute()
        {
            controller = new MockController();

            var actionContext = new ActionContext(controller.HttpContext, controller.RouteData, controller.ControllerContext.ActionDescriptor);

            authorizationFilterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        }

        [Fact]
        public void ShouldReturnUnAuthorized()
        {
            var authAttr = new AllowedUserAttribute();
            authAttr.OnAuthorization(authorizationFilterContext);

            authorizationFilterContext.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void ShouldReturnForbidden()
        {
            controller.ControllerContext.HttpContext.User = GetTestUser("test");

            var authAttr = new AllowedUserAttribute("ttcg");
            authAttr.OnAuthorization(authorizationFilterContext);

            authorizationFilterContext.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void ShouldReturnSuccessful()
        {
            controller.ControllerContext.HttpContext.User = GetTestUser("ttcg");

            var authAttr = new AllowedUserAttribute("ttcg");
            authAttr.OnAuthorization(authorizationFilterContext);

            authorizationFilterContext.Result.Should().BeNull(); // null means successful
        }

        private ClaimsPrincipal GetTestUser(string name)
        {
            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, name)
            }, "basic");

            var claimsPrincipal = new ClaimsPrincipal(identity);
            return claimsPrincipal;
        }
    }
}
