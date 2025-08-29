using CanvasAccountRegistration.Logic.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Logic.DataAccess
{
    public interface IAccountDataAccessExtended : IAccountDataAccess
    {
        Task<IEnumerable<string>> GetAccountTypes();
    }

    public class AccountDataAccessExtended : AccountDataAccess, IAccountDataAccessExtended
    {
        public AccountDataAccessExtended(ISqlDataAccess db, SqlStringBuilder<Account> sqlStringBuilder)
            : base(db, sqlStringBuilder)
        { }

        public async Task<IEnumerable<string>> GetAccountTypes()
        {
            string sql = $"SELECT DISTINCT AccountType FROM [{Table}] ";
            var accounts = await ExecuteSelectMany(sql);
            return accounts.Select(x => x.AccountType);
        }
    }
} 