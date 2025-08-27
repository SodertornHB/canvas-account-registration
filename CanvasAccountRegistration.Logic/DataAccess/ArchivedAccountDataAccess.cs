
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.DataAccess;

namespace CanvasAccountRegistration.Logic.DataAccess
{
    public interface IArchivedAccountDataAccess : IDataAccess<ArchivedAccount>
    {    }

    public class ArchivedAccountDataAccess : BaseDataAccess<ArchivedAccount>, IArchivedAccountDataAccess
    {
        public ArchivedAccountDataAccess(ISqlDataAccess db, SqlStringBuilder<ArchivedAccount> sqlStringBuilder)
            : base(db, sqlStringBuilder)
        { }
     }
} 