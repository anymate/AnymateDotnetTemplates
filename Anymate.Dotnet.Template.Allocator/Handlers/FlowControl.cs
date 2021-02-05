using Anymate.Dotnet.Template.Allocator.Configuration;
using Anymate.Dotnet.Template.Allocator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Anymate.Dotnet.Template.Allocator.Handlers
{

    public interface IFlowControl
    {
        Task Run();
    }

    public class FlowControl : IFlowControl
    {
        private readonly ILogger<FlowControl> _logger;
        private readonly IDataGatherer _dataGatherer;
        public IAnymateService _anymateService;
        private string _processKey;
        private AnymateRules Rules { get; set; }
        private long RunId { get; set; } = -1;
        public FlowControl(IOptions<AnymateConfig> anymateConfigOptions, ILogger<FlowControl> logger, IDataGatherer dataGatherer)
        {
            var anymateConfig = anymateConfigOptions.Value;
            _anymateService = new AnymateService(anymateConfig.ClientId, anymateConfig.Secret, anymateConfig.Username, anymateConfig.Password);
            _logger = logger;
            _dataGatherer = dataGatherer;
            _processKey = anymateConfig.ProcessKey;
        }

        public async Task Run()
        {
            try
            {
                await StartUp();

                var newTasks = await _dataGatherer.PrepareData(Rules);

                _logger.LogTrace($"Trying to create {newTasks.Count()} new tasks @ {_processKey}");
                var createTasksResponse = await _anymateService.CreateTasksAsync(newTasks, _processKey);
                if (!createTasksResponse.Succeeded)
                {
                    _logger.LogError("Failed to create new tasks");
                }


                await ShutDown();

            }
            catch (Exception ex)
            {
                var failure = $"Got Exception with msg: {ex.Message} // from: {ex.Source} // stackTrace: {ex.StackTrace}";
                _logger.LogError("Encountered a failure. " + failure);
                var failureResponse = await _anymateService.FailureAsync(_processKey, failure);
                if (!failureResponse.Succeeded)
                {
                    _logger.LogError("Failed to register Failure with Anymate.");
                }
                return;
            }
        }


        public async Task StartUp()
        {
            try
            {
                await InitializeAnymate();
            }
            catch (Exception ex)
            {
                var failure = $"Got Exception with msg: {ex.Message} // from: {ex.Source} // stackTrace: {ex.StackTrace}";
                _logger.LogError("Encountered a failure during startup. " + failure);
                var failureResponse = await _anymateService.FailureAsync(_processKey, failure);
                if (!failureResponse.Succeeded)
                {
                    _logger.LogError("Failed to register Failure with Anymate.");
                }
                return;
            }

        }

        public async Task ShutDown()
        {
            var finishRun = await _anymateService.FinishRunAsync(RunId);
            if (!finishRun.Succeeded)
            {
                _logger.LogWarning($"FinishRun did not succeeded {_processKey} with RunId: {RunId}");
            }

        }


        private async Task InitializeAnymate()
        {
            _logger.LogTrace("Started application...");

            var okToRun = await _anymateService.OkToRunAsync(_processKey);
            if (!okToRun.OkToRun)
            {
                _logger.LogTrace($"Not ok to start run @ {_processKey}");
                return;
            }
            _logger.LogTrace($"Ok to run @ {_processKey}");

            Rules = await _anymateService.GetRulesAsync<AnymateRules>(_processKey);
            /*
             * If you need to make data available to be shared among all Tasks, you can add it as a field in your rules model and assign it here.
             */

            var run = await _anymateService.StartOrGetRunAsync(_processKey);
            if (run.RunId < 1)
            {
                _logger.LogWarning($"Did not get a RunId on {_processKey}. Stopping the app..");
                return;
            }
            RunId = run.RunId;
        }

        private async Task<AnymateResponse> ReturnTaskToAnymate(TaskAction action)
        {
            if (action.TaskId < 1)
            {
                var failureMsg = $"TaskId was {action.TaskId} - Not possible to return Task to Anymate";
                return LogAnymateResponse(action.TaskId, "[No valid TaskId]", new AnymateResponse() { Message = failureMsg, Succeeded = false });

            }

            if (action.SendToError && action.RetryTaskLater)
            {
                var endpoint = "Retry";
                var response = await _anymateService.RetryAsync(action.GetAnymateRetryTaskAction());
                return LogAnymateResponse(action.TaskId, endpoint, response);
            }

            if (action.SendToError)
            {
                var endpoint = "Error";
                var response = await _anymateService.ErrorAsync(action.GetAnymateTaskAction());
                return LogAnymateResponse(action.TaskId, endpoint, response);
            }

            if (action.SendToManual)
            {
                var endpoint = "Manual";
                var response = await _anymateService.ManualAsync(action.GetAnymateTaskAction());
                return LogAnymateResponse(action.TaskId, endpoint, response);
            }

            {
                var endpoint = "Solved";
                var response = await _anymateService.SolvedAsync(action.GetAnymateTaskAction());
                return LogAnymateResponse(action.TaskId, endpoint, response);
            }

        }


        private AnymateResponse LogAnymateResponse(long taskId, string endpoint, AnymateResponse response)
        {
            if (!response.Succeeded)
            {
                _logger.LogError($"Task {taskId}: Did not succeeded sending task to {endpoint}. Got message: {response.Message}");
            }
            else
            {
                _logger.LogTrace($"Task {taskId}: Succeeded sending task to {endpoint}. Got message: {response.Message}");
            }
            return response;
        }
    }
}


