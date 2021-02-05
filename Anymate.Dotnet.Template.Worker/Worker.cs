using Anymate.Dotnet.Template.Worker.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Anymate.Dotnet.Template.Worker
{
    public interface IWorker
    {
        Task<TaskAction> ProcessTask(AnymateTask task);
    }
    public class Worker : IWorker
    {
        private readonly ILogger<Worker> _logger;
        private readonly AnymateRules _rules;
        public Worker(ILogger<Worker> logger, AnymateRules rules)
        {
            _logger = logger;
            _rules = rules;
        }
        // FlowControl booleans: sendToManual, sendToError, retryTaskLater
        private bool sendToManual { get; set; } = false;
        private bool sendToError { get; set; } = false;
        private bool retryTaskLater { get; set; } = false; // Retry Task Later will only be applied if Task is *also* sent to Error. If all Tasks should always be retried later if possible, then set this value to True.
        private string reason { get; set; }
        private string comment { get; set; }
        private DateTimeOffset? activationDate { get; set; } = null;
        private int? overwriteEntries { get; set; } = null;
        private int? overwriteSecondsSaved { get; set; } = null;

        public async Task<TaskAction> ProcessTask(AnymateTask task)
        {
            try
            {
                return await PerformWorkOnTask(task);
            }
            catch(Exception ex)
            {
                sendToError = true;
                retryTaskLater = true;
                reason = "Exception";
                comment = $"Got Exception with msg: {ex.Message} // from: {ex.Source} // stackTrace: {ex.StackTrace}";
                return FinishTask(task);
            }
        }

        private async Task<TaskAction> PerformWorkOnTask(AnymateTask task)
        {
            _logger.LogTrace($"Task {task.TaskId}: Started processsing Task from Process: {task.ProcessKey} as part of Run {task.RunId}");

            /*
             * Implement the worker logic here. 
             * Make sure to update reason, comment and the flow control booleans as you go.
             */


            return FinishTask(task);
        }


        // Call FinishTask when there is no more work to be done on the Task.
        private TaskAction FinishTask(AnymateTask task)
        {
            var endpoint = sendToError ?
                retryTaskLater ? "Retry" : "Error"
                : sendToManual ? "Manual" : "Solved";
            _logger.LogTrace($"Task {task.TaskId}: Sending task to {endpoint} with Reason: {reason} and Comment: {comment}.");
            

            var action = new TaskAction(task.TaskId, reason, comment, sendToManual: sendToManual, sendToError: sendToError, retryTaskLater: retryTaskLater, newActivationDate: activationDate, overwriteSecondsSaved: overwriteSecondsSaved, overwriteEntries: overwriteEntries);
            return action;
        }

    }
}
