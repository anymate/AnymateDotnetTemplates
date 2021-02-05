using System;

namespace Anymate.Dotnet.Template.Worker.Models
{
    public class TaskAction
    {
        public TaskAction()
        {
        }

        public TaskAction(long taskId, string reason = null, string comment = null, bool sendToManual = false, bool sendToError = false, bool retryTaskLater = false, DateTimeOffset? newActivationDate = null,  int? overwriteSecondsSaved = null, int? overwriteEntries = null)
        {
            SendToManual = sendToManual;
            SendToError = sendToError;
            RetryTaskLater = retryTaskLater;
            NewActivationDate = newActivationDate;
            TaskId = taskId;
            Reason = reason;
            Comment = comment;
            OverwriteSecondsSaved = overwriteSecondsSaved;
            OverwriteEntries = overwriteEntries;
        }

        public bool SendToManual { get; set; } = false;
        public bool SendToError { get; set; } = false;
        public bool RetryTaskLater { get; set; } = false;
        public DateTimeOffset? NewActivationDate { get; set; } = null;
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
                ActivationDate = this.NewActivationDate
            };
        }
    }
}
