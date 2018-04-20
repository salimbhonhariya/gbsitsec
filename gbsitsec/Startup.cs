using gbsitsec.Data;
using gbsitsec.Models;
using gbsitsec.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace gbsitsec
{
    public class Startup
    {
        private string UserSecretsId = null;
        private string googleclient_id;
        private string googleproject_id;
        private string googleauth_uri;
        private string googletoken_uri;
        private string googleauth_provider_x509_cert_url;
        private string googleclient_secret;
        private string googleredirect_uris;
        private string googlejavascript_origins;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = googleclient_id;//Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = googleclient_secret;//Configuration["Authentication:Google:ClientSecret"];
            });

            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = Configuration["Microsoft:ApplicationId"];
                microsoftOptions.ClientSecret = Configuration["Microsoft:Password"];
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                // If the LoginPath isn't set, ASP.NET Core defaults
                // the path to /Account/Login.
                options.LoginPath = "/Account/Login";
                // If the AccessDeniedPath isn't set, ASP.NET Core defaults
                // the path to /Account/AccessDenied.
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var result = string.IsNullOrEmpty(UserSecretsId) ? "Null" : "Not Null";
            if (result != null)
            {
                //var result = string.IsNullOrEmpty(_testSecret) ? "Null" : "Not Null";
                //app.Run(async (context) =>
                //{
                //    await context.Response.WriteAsync($"Secret is {result}");
                //});
                UserSecretsId = Configuration["UserSecretsId"];
                googleclient_id = Configuration["googleclient_id"];
                googleproject_id = Configuration["googleproject_id"];
                googleauth_uri = Configuration["googleauth_uri"];
                googletoken_uri = Configuration["googletoken_uri"];
                googleauth_provider_x509_cert_url = Configuration["googleauth_provider_x509_cert_url"];
                googleclient_secret = Configuration["googleclient_secret"];
                googleredirect_uris = Configuration["googleredirect_uris"];
                googlejavascript_origins = Configuration["googlejavascript_origins"];
            }

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}