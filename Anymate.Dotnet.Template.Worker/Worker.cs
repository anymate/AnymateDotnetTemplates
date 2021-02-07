using Anymate.Dotnet.Template.Worker.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Anymate.Dotnet.Template.Worker.Configuration;


namespace Anymate.Dotnet.Template.Worker
{
    public interface IWorker
    {
        Task<TaskAction> ProcessTask(AnymateTask task);
    }
    public class Worker : IWorker
    {
        private AnymateEndpoint AnymateEndpoint { get; set; } = AnymateEndpoint.Solved;
        private readonly ILogger<Worker> _logger;
        private readonly AnymateRules _rules;
        public Worker(ILogger<Worker> logger, AnymateRules rules)
        {
            _logger = logger;
            _rules = rules;
        }
        // FlowControl booleans: sendToManual, sendToError, retryTaskLater


        public async Task<TaskAction> ProcessTask(AnymateTask task)
        {
            try
            {
                return await PerformWorkOnTask(task);
            }
            catch (Exception ex)
            {
                var reason = "Exception";
                var comment = $"Got Exception with msg: {ex.Message} // from: {ex.Source} // stackTrace: {ex.StackTrace}";
                return FinishTask(task.TaskId, reason, comment, AnymateEndpoint.Retry);
            }
        }

        private async Task<TaskAction> PerformWorkOnTask(AnymateTask task)
        {
            _logger.LogTrace($"Task {task.TaskId}: Started processsing Task from Process: {task.ProcessKey} as part of Run {task.RunId}");

            /*
             * Implement the worker logic here. 
             * Make sure to update reason, comment and the flow control booleans as you go.
             */

            var reason = "Task Solved";
            var comment = "Finished processing the Task";
            return FinishTask(task.TaskId, reason, comment, endpoint: AnymateEndpoint);
        }


        // Call FinishTask when there is no more work to be done on the Task.
        private TaskAction FinishTask(long taskId, string reason = null, string comment = null, AnymateEndpoint endpoint = AnymateEndpoint.Solved, DateTimeOffset? activationDate = null, int? overwriteSecondsSaved = null, int? overwriteEntries = null)
        {
            return endpoint switch
            {
                AnymateEndpoint.Manual => SendTaskToManual(taskId, reason, comment, overwriteSecondsSaved: overwriteSecondsSaved, overwriteEntries: overwriteEntries),
                AnymateEndpoint.Retry => SendTaskToRetry(taskId, reason, comment, overwriteSecondsSaved: overwriteSecondsSaved, overwriteEntries: overwriteEntries, activationDate: activationDate),
                AnymateEndpoint.Error => SendTaskToError(taskId, reason, comment, overwriteSecondsSaved: overwriteSecondsSaved, overwriteEntries: overwriteEntries),
                _ => SendTaskToSolved(taskId, reason, comment, overwriteSecondsSaved: overwriteSecondsSaved, overwriteEntries: overwriteEntries),
            };
        }
        private TaskAction SendTaskToRetry(long taskId, string reason = null, string comment = null, DateTimeOffset? activationDate = null, int? overwriteSecondsSaved = null, int? overwriteEntries = null)
        {
            var endpoint = "Retry";
            _logger.LogTrace($"Task {taskId}: Sending task to {endpoint} with Reason: {reason} and Comment: {comment}.");


            var action = new TaskAction(taskId, reason, comment, endpoint: AnymateEndpoint.Retry, activationDate: activationDate, overwriteSecondsSaved: overwriteSecondsSaved, overwriteEntries: overwriteEntries);
            return action;
        }

        private TaskAction SendTaskToManual(long taskId, string reason = null, string comment = null, int? overwriteSecondsSaved = null, int? overwriteEntries = null)
        {
            var endpoint = "Manual";
            _logger.LogTrace($"Task {taskId}: Sending task to {endpoint} with Reason: {reason} and Comment: {comment}.");


            var action = new TaskAction(taskId, reason, comment, endpoint: AnymateEndpoint.Manual, overwriteSecondsSaved: overwriteSecondsSaved, overwriteEntries: overwriteEntries);
            return action;
        }

        private TaskAction SendTaskToError(long taskId, string reason = null, string comment = null, int? overwriteSecondsSaved = null, int? overwriteEntries = null)
        {
            var endpoint = "Error";
            _logger.LogTrace($"Task {taskId}: Sending task to {endpoint} with Reason: {reason} and Comment: {comment}.");


            var action = new TaskAction(taskId, reason, comment, endpoint: AnymateEndpoint.Error, overwriteSecondsSaved: overwriteSecondsSaved, overwriteEntries: overwriteEntries);
            return action;
        }

        private TaskAction SendTaskToSolved(long taskId, string reason = null, string comment = null, int? overwriteSecondsSaved = null, int? overwriteEntries = null)
        {
            var endpoint = "Solved";
            _logger.LogTrace($"Task {taskId}: Sending task to {endpoint} with Reason: {reason} and Comment: {comment}.");


            var action = new TaskAction(taskId, reason, comment, endpoint: AnymateEndpoint.Solved, overwriteSecondsSaved: overwriteSecondsSaved, overwriteEntries: overwriteEntries);
            return action;
        }

    }
}
