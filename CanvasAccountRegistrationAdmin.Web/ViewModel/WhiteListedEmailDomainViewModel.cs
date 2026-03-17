using System;
using System.ComponentModel.DataAnnotations;

namespace CanvasAccountRegistration.Web.ViewModel
{
    public class WhiteListedEmailDomainViewModel : ViewModelBase
    {
        [Required]
        public string Domain { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
