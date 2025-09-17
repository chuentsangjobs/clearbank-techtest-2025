using ClearBank.DeveloperTest.Config;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data
{
    public interface IAccountDataStore
    {
        DataStoreType Type { get; }

        Account GetAccount(string accountNumber);

        void UpdateAccount(Account account);
    }
}
