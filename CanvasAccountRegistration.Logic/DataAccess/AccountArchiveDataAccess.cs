
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.DataAccess;

namespace CanvasAccountRegistration.Logic.DataAccess
{
    public interface IAccountArchiveDataAccess : IDataAccess<AccountArchive>
    {    }

    public class AccountArchiveDataAccess : BaseDataAccess<AccountArchive>, IAccountArchiveDataAccess
    {
        public AccountArchiveDataAccess(ISqlDataAccess db, SqlStringBuilder<AccountArchive> sqlStringBuilder)
            : base(db, sqlStringBuilder)
        { }
     }
} 