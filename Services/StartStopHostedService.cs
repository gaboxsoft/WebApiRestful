using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAppBooks.Services
{
    public class StartStopHostedService : IHostedService
    {
        private readonly IHostEnvironment environment;
        private readonly string file = "StartStopHostingService.log";
        public StartStopHostedService(IHostEnvironment environment)
        {
            this.environment = environment;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            WriteToFile($"{DateTime.Now} -> Process starting.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            WriteToFile($"{DateTime.Now} -> Process stopping.");
            return Task.CompletedTask;
        }

        public void WriteToFile(string mensaje)
        {
            var path = $@"{environment.ContentRootPath}\logs\{file}";
            using (StreamWriter writer = new StreamWriter(path, append: true)) 
            {
                writer.WriteLine(mensaje);
            };
        }
    }
}
