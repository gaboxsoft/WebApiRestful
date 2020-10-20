using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAppBooks.Services
{
    public class StartStopHostedService : IHostedService, IDisposable
    {
        private readonly IHostEnvironment environment;
        private readonly string file = "StartStopHostingService.log";
        Timer timer;

        public StartStopHostedService(IHostEnvironment environment)
        {
            this.environment = environment;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5));
            WriteToFile($"{DateTime.Now} -> Process starting.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            WriteToFile($"{DateTime.Now} -> Process stopping.");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        private void DoWork(object state)
        {
            WriteToFile($"{DateTime.Now} -> Doing some work.");
        }


        public void WriteToFile(string mensaje)
        {
            var path = $@"{environment.ContentRootPath}\logs\{file}";
            using (StreamWriter writer = new StreamWriter(path, append: true)) 
            {
                writer.WriteLine(mensaje);
            };
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
