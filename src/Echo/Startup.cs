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
using System.Net;

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

        app.UseStatusCodePagesWithReExecute("/pages/echo", "?statusCode={0}");

        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders =
                ForwardedHeaders.XForwardedHost |
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
        };

        var knownProxy = Configuration["CUSTOM_KNOWN_PROXY"];
        if (!string.IsNullOrEmpty(knownProxy))
        {
            options.KnownProxies.Add(IPAddress.Parse(knownProxy));
        }

        var allowAllProxies = Configuration["CUSTOM_ALLOW_ALL_PROXIES"];
        if (!string.IsNullOrEmpty(allowAllProxies) && bool.TryParse(allowAllProxies, out var value) && value)
        {
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
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
