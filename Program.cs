using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebAppTest01
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
        #region //-- Configuración de proveedores de datos de configuración de la WebApp
            .ConfigureAppConfiguration(
                (env, config) =>
                    {
                        var ambiente = env.HostingEnvironment.EnvironmentName;
                        config.AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
                        config.AddJsonFile($"appSettings.{ambiente}.json", optional: true, reloadOnChange: true);
                        config.AddEnvironmentVariables();
                        if (args != null)
                        {
                            config.AddCommandLine(args);
                        }
                    }
                )
        #endregion //-- Configuración de proveedores de datos de configuración de la WebApp

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
