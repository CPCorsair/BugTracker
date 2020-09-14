using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Authorization
{
    public class ProjectManagerAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Project>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement, 
                                                       Project resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Constants.ProjectManagersRole) && context.User.HasClaim("AssignedProject", resource.Id.ToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
