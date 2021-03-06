﻿using System;
using System.Security.Claims;
using BugTracker.Areas.Identity.Data;
using BugTracker.Authorization;
using BugTracker.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(BugTracker.Areas.Identity.IdentityHostingStartup))]
namespace BugTracker.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AuthDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AuthDbContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AuthDbContext>();

                services.AddAuthorization(options =>
                {
                    options.FallbackPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                });

                services.AddScoped<IAuthorizationHandler, ProjectCreatorIsOwnerAuthorizationHandler>();
                services.AddScoped<ClaimsPrincipal>();
                //is added as singleton b/c EF core isn't used and all info needed is in the Context parameter of the HandleRequirementAsync method
                services.AddSingleton<IAuthorizationHandler, ProjectManagerAuthorizationHandler>();
                services.AddSingleton<IAuthorizationHandler, ProjectDeveloperAuthorizationHandler>();
            });
        }
    }
}