using System;

namespace CanvasAccountRegistration.Logic.Model
{
    public class WhiteListedEmailDomain : Entity
    {
        public string Domain { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
