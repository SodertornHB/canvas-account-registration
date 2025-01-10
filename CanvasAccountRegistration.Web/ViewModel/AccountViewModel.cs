
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
        public virtual string login_attribute {get;set;}  = ""; 
        public virtual string given_name {get;set;}  = ""; 
        public virtual string surname {get;set;}  = ""; 
        public virtual string display_name {get;set;}  = ""; 
        public virtual string email {get;set;}  = ""; 
        public virtual string assurance_level {get;set;}  = ""; 
        public virtual bool mail_verified {get;set;} 
        [DataType(DataType.Text)]
        public virtual DateTime? create_date_time {get;set;} 
        [DataType(DataType.Text)]
        public virtual DateTime? verification_date_time {get;set;} 
        [DataType(DataType.Text)]
        public virtual DateTime? integration_date_time {get;set;} 
        public virtual string GetBackToListLink(string applicationName) => $"/{applicationName}/{GetType().Name.Replace("ViewModel","")}";
    }
} 