
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.IO;
using Web;

namespace CanvasAccountRegistration.Web
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<StartupExtended>();
                webBuilder.ConfigureKestrel(options =>
                {
                });
            })
            .ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .UseNLog();

        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("init main");
                var webHost = CreateHostBuilder(args)
                  .UseContentRoot(Directory.GetCurrentDirectory())
                  .ConfigureAppConfiguration((hostingContext, config) =>
                  {
                      var env = hostingContext.HostingEnvironment;
                      config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                                optional: true, reloadOnChange: true);
                      config.AddEnvironmentVariables();
                      try
                      {
                          var logFactory = NLog.LogManager.LoadConfiguration($"nlog.{env.EnvironmentName}.config");
                          NLog.LogManager.Configuration = logFactory.Configuration;
                      }
                      catch (Exception)
                      {
                          // No problems here!
                          // If environment specific log config doesn't exists just continue
                      }
                  })
                  .ConfigureLogging((hostingContext, logging) =>
                  {
                      logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                  })
                  .Build();

                webHost.Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }
    }
}
