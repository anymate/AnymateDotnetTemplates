using Anymate.Dotnet.Template.Worker.Configuration;
using System;

namespace Anymate.Dotnet.Template.Worker.Models
{
    public class TaskAction
    {
        public TaskAction()
        {
        }

        public TaskAction(long taskId, string reason = null, string comment = null, AnymateEndpoint endpoint = AnymateEndpoint.Solved, DateTimeOffset? activationDate = null,  int? overwriteSecondsSaved = null, int? overwriteEntries = null)
        {
            AnymateEndpoint = endpoint;
            ActivationDate = activationDate;
            TaskId = taskId;
            Reason = reason;
            Comment = comment;
            OverwriteSecondsSaved = overwriteSecondsSaved;
            OverwriteEntries = overwriteEntries;
        }
        public AnymateEndpoint AnymateEndpoint { get; set; } = AnymateEndpoint.Solved;
        public DateTimeOffset? ActivationDate { get; set; } = null;
        public long TaskId { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
        public int? OverwriteSecondsSaved { get; set; } = null;
        public int? OverwriteEntries { get; set; } = null;

        public AnymateTaskAction GetAnymateTaskAction()
        {
            return new AnymateTaskAction()
            {
                TaskId = this.TaskId,
                Comment = this.Comment,
                OverwriteEntries = this.OverwriteEntries,
                OverwriteSecondsSaved = this.OverwriteSecondsSaved,
                Reason = this.Reason
            };
        }
        public AnymateRetryTaskAction GetAnymateRetryTaskAction()
        {
            return new AnymateRetryTaskAction()
            {
                TaskId = this.TaskId,
                Comment = this.Comment,
                OverwriteEntries = this.OverwriteEntries,
                OverwriteSecondsSaved = this.OverwriteSecondsSaved,
                Reason = this.Reason,
                ActivationDate = this.ActivationDate
            };
        }
    }
}
