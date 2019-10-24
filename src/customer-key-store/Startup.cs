using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace customerkeystore
{
    using ippw = Microsoft.InformationProtection.Web.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });


            services.AddSingleton<ippw.IKeyStore, ippw.TestKeyStore>();
            services.AddTransient<ippw.KeyManager, ippw.KeyManager>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => 
            { 
                Configuration.Bind("AzureAd", options);
                options.Audience = Configuration["JwtAudience"];
                options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.Response.Headers.Add("resource", options.Audience);
                        context.Response.Headers.Add("authorization", Configuration["JwtAuthorization"]);

                        return Task.CompletedTask;
                    }                    
                };
            });
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "DecryptRoute",
                    template: "{keyName}/{keyId}/Decrypt",
                    defaults: new { controller = "Keys", action = "Decrypt" });

                routes.MapRoute(
                    name: "GetKeyRoute",
                    template: "{keyName}",
                    defaults: new { controller = "Keys", action = "GetKey" });
            });
        }
    }
}
