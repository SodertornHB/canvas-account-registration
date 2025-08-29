
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using System;

namespace CanvasAccountRegistration.Logic.Model
{
    public partial class ArchivedAccount : Entity
    {
        public virtual int InitialId {get;set;}
        public virtual string UserId {get;set;}
        public virtual string EmailDomain {get;set;}
        public virtual DateTime? CreatedOn {get;set;}
        public virtual DateTime? VerifiedOn {get;set;}
        public virtual DateTime? IntegratedOn {get;set;}
        public virtual DateTime? DeletedOn {get;set;}
        public virtual string ArchivedAccountType {get;set;}
        public virtual string ArchivedAccountRole {get;set;}
      
    }
} 