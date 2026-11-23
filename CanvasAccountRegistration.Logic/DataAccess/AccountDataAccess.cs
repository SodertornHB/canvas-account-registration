
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.DataAccess;

namespace CanvasAccountRegistration.Logic.DataAccess
{
    public interface IAccountDataAccess : IDataAccess<Account>
    {    }

    public class AccountDataAccess : BaseDataAccess<Account>, IAccountDataAccess
    {
        public AccountDataAccess(ISqlDataAccess db, SqlStringBuilder<Account> sqlStringBuilder)
            : base(db, sqlStringBuilder)
        { }
     }
} 