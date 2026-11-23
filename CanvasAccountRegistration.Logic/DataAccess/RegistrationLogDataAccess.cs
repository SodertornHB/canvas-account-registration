
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.DataAccess;

namespace CanvasAccountRegistration.Logic.DataAccess
{
    public interface IRegistrationLogDataAccess : IDataAccess<RegistrationLog>
    {    }

    public class RegistrationLogDataAccess : BaseDataAccess<RegistrationLog>, IRegistrationLogDataAccess
    {
        public RegistrationLogDataAccess(ISqlDataAccess db, SqlStringBuilder<RegistrationLog> sqlStringBuilder)
            : base(db, sqlStringBuilder)
        { }
     }
} 