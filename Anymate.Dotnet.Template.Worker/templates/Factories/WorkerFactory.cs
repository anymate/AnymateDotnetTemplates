using System;
using Anymate.Dotnet.Template.Worker.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Anymate.Dotnet.Template.Worker.Factories
{
    public interface IWorkerFactory
    {
        IWorker GetWorker(AnymateRules rules);
    }
    public class WorkerFactory : IWorkerFactory
    {
        private IServiceProvider _serviceProvider;
        public WorkerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IWorker GetWorker(AnymateRules rules)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Worker>>();
            var worker = new Worker(logger, rules);
            return worker;
        }
    }
}
