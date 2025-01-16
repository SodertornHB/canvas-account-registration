
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using System;
using System.ComponentModel.DataAnnotations;

namespace CanvasAccountRegistration.Web.ViewModel
{
    public partial class AccountViewModel : ViewModelBase
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
        public virtual DateTime? IntegratedOn {get;set;} 
        public virtual string GetBackToListLink(string applicationName) => $"/{applicationName}/{GetType().Name.Replace("ViewModel","")}";
    }
} 