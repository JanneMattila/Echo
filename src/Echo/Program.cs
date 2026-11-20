using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Echo;

public class Program
{
    public static DateTime Started = DateTime.UtcNow;

    public static void Main(string[] args)
    {
        BuildHost(args).Run();
    }

    public static IHost BuildHost(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .Build();
}
