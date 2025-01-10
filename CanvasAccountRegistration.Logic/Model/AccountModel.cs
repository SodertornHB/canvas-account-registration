
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using System;

namespace CanvasAccountRegistration.Logic.Model
{
    public partial class Account : Entity
    {
        public virtual string login_attribute {get;set;}
        public virtual string given_name {get;set;}
        public virtual string surname {get;set;}
        public virtual string display_name {get;set;}
        public virtual string email {get;set;}
        public virtual string assurance_level {get;set;}
        public virtual bool mail_verified {get;set;}
        public virtual DateTime? create_date_time {get;set;}
        public virtual DateTime? verification_date_time {get;set;}
        public virtual DateTime? integration_date_time {get;set;}
      
    }
} 