using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CanvasAccountRegistration.Web.ViewModel
{
    public partial class RegistrationViewModel : ViewModelBase
    {
        public virtual string UserId {get;set;}  = ""; 
        public virtual string DisplayName {get;set;}  = ""; 
        public virtual string GivenName {get;set;}  = ""; 
        public virtual string Surname {get;set;}  = ""; 
        public virtual string Email {get;set;}  = ""; 
        public virtual string AssuranceLevel {get;set;}  = "";
     
        [DataType(DataType.Text)]
        public virtual DateTime? CreatedOn {get;set;} 
        [DataType(DataType.Text)]
        public virtual DateTime? VerifiedOn {get;set;} 
        [DataType(DataType.Text)]
        public virtual DateTime? IntegratedOn {get;set; }
        public string[] AssuranceLevels { get; set; }
        public virtual string SwamidAssuranceLevel { get; set; }
        public virtual string IdentityAssuranceProfile { get; set; }
        public bool IsVerifiedWithId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsIntegrated { get; set; }

    }
} 