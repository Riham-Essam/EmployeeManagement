using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;

            }).AddEntityFrameworkStores<AppDbContext>();
            services.AddMvc(options =>
           {
               var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
               options.Filters.Add(new AuthorizeFilter(policy));
           }).AddXmlSerializerFormatters();
            services.AddAuthorization(options =>
            {

                //options.AddPolicy("EditRolePolicy", policy =>
                // policy.RequireAssertion(context =>
                // context.User.IsInRole("Admin") &&
                // context.User.HasClaim(cliam => cliam.Type == "Edit Role" && cliam.Value == "true") ||
                // context.User.IsInRole("Super Admin")
                //));

                //options.AddPolicy("EditRolePolicy", policy =>
                //policy.RequireAssertion(context => AuthorizeAccess(context)));

                options.AddPolicy("EditRolePolicy", policy =>
               policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));


                options.AddPolicy("DeleteRolePolicy", policy =>
                 policy.RequireClaim("Delete Role", "true"));


                options.AddPolicy("AdminRolePolicy", policy =>
                policy.RequireRole("Admin"));
            });

            services.ConfigureApplicationCookie(options =>
            options.AccessDeniedPath = new PathString("/Administration/AccessDenied"));

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
        }


        private bool AuthorizeAccess(AuthorizationHandlerContext context)
        {
            return context.User.IsInRole("Admin") && context.User.HasClaim(c => c.Type == "Edit Role" &&
            c.Value == "true") || context.User.IsInRole("Super Admin");
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            //FileServerOptions fo = new FileServerOptions();
            //fo.DefaultFilesOptions.DefaultFileNames.Clear();
            //fo.DefaultFilesOptions.DefaultFileNames.Add("foo.html");
            app.UseFileServer();
            //app.UseMvcWithDefaultRoute();

            app.UseAuthentication();
            app.UseMvc(Routes =>
            {
                Routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
