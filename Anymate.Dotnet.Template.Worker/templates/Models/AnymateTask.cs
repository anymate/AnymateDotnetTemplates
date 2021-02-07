namespace Anymate.Dotnet.Template.Worker.Models
{
    public class AnymateTask
    {
        public long TaskId { get; set; } //The TaskId
        public string Reason { get; set; } //The current Reason from the Task
        public long RunId { get; set; } //The Id of the current Process Run
        public string ProcessKey { get; set; } //The ProcessKey where the Task belong

        /* 
         * Add your own parameters here depending on how the Process is configured.
         */

    }
}
