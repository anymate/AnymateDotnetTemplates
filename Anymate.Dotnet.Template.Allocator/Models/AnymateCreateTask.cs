using System;

namespace Anymate.Dotnet.Template.Worker.Models
{
    public class AnymateCreateTask
    {
        public DateTimeOffset? ActivationDate { get; set; }
        public string Comment { get; set; }

        /* 
         * Add your own parameters here depending on how the Process is configured.
         */

    }
}
