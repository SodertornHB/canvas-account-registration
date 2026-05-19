
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using System;

namespace CanvasAccountRegistration.Logic.Model
{
    public partial class Account : Entity
    {
        public virtual string UserId {get;set;}
        public virtual string DisplayName {get;set;}
        public virtual string GivenName {get;set;}
        public virtual string Surname {get;set;}
        public virtual string Email {get;set;}
        public virtual string AssuranceLevel {get;set;}
        public virtual DateTime? CreatedOn {get;set;}
        public virtual DateTime? VerifiedOn {get;set;}
        public virtual DateTime? IntegratedOn {get;set;}
        public virtual string AccountType {get;set;}
        public virtual string AccountRole {get;set;}
      
    }
} 