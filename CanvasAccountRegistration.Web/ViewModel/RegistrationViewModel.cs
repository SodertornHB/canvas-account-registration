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
        public string[] AssuranceLevels => AssuranceLevel?.Replace(" ","").Split(',') ?? Array.Empty<string>();

        public virtual string SwamidAssuranceLevel =>
            AssuranceLevels
                .SingleOrDefault(x => x.StartsWith("http://www.swamid.se/policy/assurance/"))?
                .Substring("http://www.swamid.se/policy/assurance/".Length);
        public virtual string IdentityAssuranceProfile =>
            AssuranceLevels
                .SingleOrDefault(x => x.StartsWith("https://refeds.org/assurance/IAP/"))?
                .Substring("https://refeds.org/assurance/IAP/".Length);

        [DataType(DataType.Text)]
        public virtual DateTime? CreatedOn {get;set;} 
        [DataType(DataType.Text)]
        public virtual DateTime? VerifiedOn {get;set;} 
        [DataType(DataType.Text)]
        public virtual DateTime? IntegratedOn {get;set; }
        public bool IsVerifiedWithId => SwamidAssuranceLevel == "al2" && (IdentityAssuranceProfile == "medium" || IdentityAssuranceProfile == "high");
        public bool IsApproved => VerifiedOn != null;
        public bool IsIntegrated => IntegratedOn != null;
    }
} 