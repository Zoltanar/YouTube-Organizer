using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using YoutubeOrganizer.Data;
using YoutubeOrganizer.Models;
using YoutubeOrganizer.Services;

namespace YoutubeOrganizer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            var googleOptions = new GoogleOptions
            {
                AccessType = "offline",
                SaveTokens = true,
                Scope = { YouTubeService.Scope.Youtube },
                AuthenticationScheme = "Google"

            };
            using (var stream = new FileStream(GlobalVariables.SecretsFile, FileMode.Open, FileAccess.Read))
            {
                ClientSecrets secrets = GoogleClientSecrets.Load(stream).Secrets;
                googleOptions.ClientId = secrets.ClientId;
                googleOptions.ClientSecret = secrets.ClientSecret;
            }
            GlobalVariables.UserLoginInfo = new Dictionary<string, ExternalLoginInfo>();
            if (File.Exists(GlobalVariables.TempSavedLoginInfoFile))
            {
                using (
                    var reader =
                        new StreamReader(new FileStream(GlobalVariables.TempSavedLoginInfoFile, FileMode.Open,
                            FileAccess.Read)))
                {
                    string jsonContent = reader.ReadToEnd();
                    if (jsonContent.Length == 0)
                    {
                        GlobalVariables.UserLoginInfo = new Dictionary<string, ExternalLoginInfo>();
                    }
                    else
                    {
                        try
                        {
                            GlobalVariables.UserLoginInfo =
                                JsonConvert.DeserializeObject<Dictionary<string, ExternalLoginInfo>>(jsonContent);
                        }
                        catch (JsonSerializationException)
                        {
                            GlobalVariables.UserLoginInfo = new Dictionary<string, ExternalLoginInfo>();
                        }
                    }
                }
            }
            app.UseGoogleAuthentication(googleOptions);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
