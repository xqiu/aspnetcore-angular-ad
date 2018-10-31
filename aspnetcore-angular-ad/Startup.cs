using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using MyMisWeb.Data;
using MyMisWeb.Filters;
using MyMisWeb.Models;
using MyMisWeb.Options;
using MyMisWeb.Services;
using System;

namespace MyMisWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MyMisContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc(opts =>
            {
                opts.Filters.Add(typeof(AdalTokenAcquisitionExceptionFilter));
            }).AddJsonOptions(options =>
            {
                //options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            });

            //TODO: Set up Data Protection key persistence correctly for your env: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?tabs=aspnetcore2x
            //I go with defaults, which works fine in my case
            //But if you run on Azure App Service and use deployment slots, keys get swapped with the app
            //So you'll need to setup storage for keys outside the app, Key Vault and Blob Storage are some options
            services.AddDataProtection();

            //Add a strongly-typed options class to DI
            services.Configure<AuthOptions>(Configuration.GetSection("Authentication"));

            services.AddSingleton<ITokenCacheFactory, TokenCacheFactory>();
            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddTransient<IEmailService, EmailService>();

            services.AddAuthentication(auth =>
            {
                //auth.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; //JwtBearerDefaults.AuthenticationScheme
                auth.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //auth.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(opts =>
            {
                Configuration.GetSection("Authentication").Bind(opts);

                opts.Events = new OpenIdConnectEvents
                {
                    OnAuthorizationCodeReceived = async ctx =>
                    {
                        HttpRequest request = ctx.HttpContext.Request;
                        //We need to also specify the redirect URL used
                        string currentUri = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path);
                        //Credentials for app itself
                        var credential = new ClientCredential(ctx.Options.ClientId, ctx.Options.ClientSecret);

                        //Construct token cache
                        ITokenCacheFactory cacheFactory = ctx.HttpContext.RequestServices.GetRequiredService<ITokenCacheFactory>();
                        TokenCache cache = cacheFactory.CreateForUser(ctx.Principal);

                        var authContext = new AuthenticationContext(ctx.Options.Authority, cache);

                        //Get token for Microsoft Graph API using the authorization code
                        string resource = "https://graph.microsoft.com";
                        AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(
                            ctx.ProtocolMessage.Code, new Uri(currentUri), credential, resource);

                        //Tell the OIDC middleware we got the tokens, it doesn't need to do anything
                        ctx.HandleCodeRedemption(result.AccessToken, result.IdToken);
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
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //Outside dev, require HTTPS and use HSTS
                app.Use(async (ctx, next) =>
                {
                    if (!ctx.Request.IsHttps)
                    {
                        //Insecure request, redirect to HTTPS side
                        HttpRequest req = ctx.Request;
                        string url = "https://" + req.Host + req.Path + req.QueryString;
                        ctx.Response.Redirect(url, permanent: true);
                    }
                    else
                    {
                        //Apply Strict Transport Security to all secure requests
                        //All requests done over secure channel for next year
                        ctx.Response.Headers["Strict-Transport-Security"] = "max-age=31536000";

                        await next();
                    }
                });
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
