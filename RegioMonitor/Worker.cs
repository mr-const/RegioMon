using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RegioMon
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(
                ILogger<Worker> logger
            )
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Regiojet Monitor worker started");
            return Task.CompletedTask;
        }
    }
}
