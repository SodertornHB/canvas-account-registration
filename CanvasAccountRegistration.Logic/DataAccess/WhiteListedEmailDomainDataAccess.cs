using CanvasAccountRegistration.Logic.Model;

namespace CanvasAccountRegistration.Logic.DataAccess
{
    public interface IWhiteListedEmailDomainDataAccess : IDataAccess<WhiteListedEmailDomain> { }

    public class WhiteListedEmailDomainDataAccess : BaseDataAccess<WhiteListedEmailDomain>, IWhiteListedEmailDomainDataAccess
    {
        public WhiteListedEmailDomainDataAccess(ISqlDataAccess db, SqlStringBuilder<WhiteListedEmailDomain> sqlStringBuilder)
            : base(db, sqlStringBuilder)
        { }
    }
}
