using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Echo;

public class Program
{
    public static DateTime Started = DateTime.UtcNow;

    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
}
