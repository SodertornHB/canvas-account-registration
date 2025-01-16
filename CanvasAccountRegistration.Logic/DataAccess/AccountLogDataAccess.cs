
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.DataAccess;

namespace CanvasAccountRegistration.Logic.DataAccess
{
    public interface IAccountLogDataAccess : IDataAccess<AccountLog>
    {    }

    public class AccountLogDataAccess : BaseDataAccess<AccountLog>, IAccountLogDataAccess
    {
        public AccountLogDataAccess(ISqlDataAccess db, SqlStringBuilder<AccountLog> sqlStringBuilder)
            : base(db, sqlStringBuilder)
        { }
     }
} 