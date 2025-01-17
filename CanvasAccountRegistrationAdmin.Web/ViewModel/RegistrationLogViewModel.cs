
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using System;
using System.ComponentModel.DataAnnotations;

namespace CanvasAccountRegistration.Web.ViewModel
{
    public partial class RegistrationLogViewModel : ViewModelBase
    {
        public virtual string displayName {get;set;}  = ""; 
        public virtual string givenName {get;set;}  = ""; 
        public virtual string sn {get;set;}  = ""; 
        public virtual string eduPersonPrincipalName {get;set;}  = ""; 
        public virtual string mail {get;set;}  = ""; 
        public virtual string eduPersonAssurance {get;set;}  = ""; 
        [DataType(DataType.Text)]
        public virtual DateTime? CreatedOn {get;set;} 
        public virtual string GetBackToListLink(string applicationName) => $"/{applicationName}/{GetType().Name.Replace("ViewModel","")}";
    }
} 