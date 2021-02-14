using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace FilesManagement.Api.Security
{
    public class AllowedUserAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public List<string> AllowedUserNames;

        public AllowedUserAttribute(params string[] usernames)
        {
            AllowedUserNames = usernames.ToList();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var user = context.HttpContext.User;            
                
            if (!AllowedUserNames.Contains(user.Identity.Name))
            {
                context.Result = new ForbidResult();
                return;
            }            
        }
    }
}
