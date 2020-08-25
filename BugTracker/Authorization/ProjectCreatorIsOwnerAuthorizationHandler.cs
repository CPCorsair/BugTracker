using BugTracker.Areas.Identity.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Authorization
{
    public class ProjectCreatorIsOwnerAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Project>
    {
        UserManager<ApplicationUser> _userManager;

        public ProjectCreatorIsOwnerAuthorizationHandler(UserManager<ApplicationUser>
           userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
                                                       OperationAuthorizationRequirement requirement, 
                                                       Project resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.

            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            if (resource.OwnerId == _userManager.GetUserId(context.User))
            {
                //returns context.Succeed if current authenticated user is the project owner
                context.Succeed(requirement);
            }

            //returns Task.CompletedTask when requirements aren't met (current authenticated user isn't the project owner)
            return Task.CompletedTask;
        }
    }
}
