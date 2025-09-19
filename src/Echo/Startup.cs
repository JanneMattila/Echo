using Echo.Hubs;
using Echo.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace Echo;

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
        services.AddResponseCompression(options =>
        {
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.AddHealthChecks();
        services.AddSignalR();
        services
            .AddControllersWithViews()
            .AddControllersAsServices();

        services.Configure<EchoOptions>(Configuration.GetSection("Echo"));

        // Swagger / OpenAPI
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<AddRawBodyParameterFilter>();
            options.SwaggerDoc("v1", new() 
            { 
                Title = "Echo API", 
                Version = "v1", 
                Description = "Echo endpoints",
                License = new OpenApiLicense
                {
                    Name = "Use under MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT"),
                }
            });

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(System.AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var pathBase = Configuration["CUSTOM_PATH"];
        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(new PathString(pathBase));
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        // Enable middleware to serve generated Swagger as JSON and the UI (placed early so it's accessible even if other middleware fails later)
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Echo API v1");
            c.RoutePrefix = "swagger"; // access at /swagger
        });

        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders =
                ForwardedHeaders.XForwardedHost |
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
        };

        var allowAllProxies = Configuration["CUSTOM_ALLOW_ALL_PROXIES"];
        if (!string.IsNullOrEmpty(allowAllProxies) && bool.TryParse(allowAllProxies, out var value) && value)
        {
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        }

        var knownProxy = Configuration["CUSTOM_KNOWN_PROXY"];
        if (!string.IsNullOrEmpty(knownProxy))
        {
            options.KnownProxies.Add(IPAddress.Parse(knownProxy));
        }

        var allowedHost = Configuration["CUSTOM_ALLOWED_HOST"];
        if (!string.IsNullOrEmpty(allowedHost))
        {
            options.AllowedHosts.Add(allowedHost);
        }

        var forwardedHostHeader = Configuration["CUSTOM_FORWARDED_HOST_HEADER"];
        if (!string.IsNullOrEmpty(forwardedHostHeader))
        {
            options.ForwardedHostHeaderName = forwardedHostHeader;
        }

        app.UseForwardedHeaders(options);

        app.UseStatusCodePagesWithReExecute("/pages/echo", "?statusCode={0}");

        app.UseResponseCompression();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/healthz");
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Pages}/{action=Index}/{id?}");
            endpoints.MapHub<EchoHub>("Echo");
        });
    }
}
