using System;
using System.ComponentModel.DataAnnotations;

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
        public bool IsVerifiedWithId => AssuranceLevel.Contains("high", StringComparison.OrdinalIgnoreCase) ||
                                        AssuranceLevel.Contains("medium", StringComparison.OrdinalIgnoreCase) ||
                                        AssuranceLevel.Contains("Al2", StringComparison.OrdinalIgnoreCase);
        public bool IsApproved => VerifiedOn != null;
        public bool IsIntegrated => IntegratedOn != null;
    }
} 