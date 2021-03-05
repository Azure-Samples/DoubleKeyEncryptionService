// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace CustomerKeyStore
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using ippw = Microsoft.InformationProtection.Web.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });
            #if USE_TEST_KEYS
            #error !!!!!!!!!!!!!!!!!!!!!! Use of test keys is only supported for testing, DO NOT USE FOR PRODUCTION !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            services.AddSingleton<ippw.IKeyStore, ippw.TestKeyStore>();
            #endif
            
            services.AddTransient<ippw.KeyManager, ippw.KeyManager>();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllers().AddNewtonsoftJson();

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
                options.Challenge = "Bearer resource=\"" + Configuration["JwtAudience"] + "\", authorization=\"" + Configuration["JwtAuthorization"] + "\", realm=\"" + Configuration["JwtAudience"] + "\"";

                var proxyConfig = Configuration.GetSection("Proxy");
                if(proxyConfig != null && proxyConfig.Exists())
                {
                    options.BackchannelHttpHandler = new System.Net.Http.HttpClientHandler
                    {
                        UseProxy = true,
                        Proxy = new System.Net.WebProxy
                        {
                            Address = new System.Uri(proxyConfig["address"]),
                            BypassProxyOnLocal = true,
                            UseDefaultCredentials = true,
                        },
                    };
                }

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.Response.Headers.Add("resource", options.Audience);
                        context.Response.Headers.Add("authorization", Configuration["JwtAuthorization"]);

                        return Task.CompletedTask;
                    },
                };
            });
        }
    }
}
