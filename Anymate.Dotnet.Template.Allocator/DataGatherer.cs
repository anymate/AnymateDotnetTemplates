using Anymate.Dotnet.Template.Allocator.Models;
using Anymate.Dotnet.Template.Worker.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Anymate.Dotnet.Template.Allocator
{

    public interface IDataGatherer
    {
        Task<IEnumerable<AnymateCreateTask>> PrepareData(AnymateRules rules);
    }
    public class DataGatherer : IDataGatherer
    {
        private readonly ILogger<DataGatherer> _logger;

        public DataGatherer(ILogger<DataGatherer> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<AnymateCreateTask>> PrepareData(AnymateRules rules)
        {
            _logger.LogTrace($"Gathering data for task creation");
            var newTasks = new List<AnymateCreateTask>();

            /*
             * Implement custom logic here to access sql servers, csv files, ftps, webservices or other data sources
             * Make sure to convert the data and add it to your AnymateCreateTask list.
             */

            return newTasks;
        }

    }
}
