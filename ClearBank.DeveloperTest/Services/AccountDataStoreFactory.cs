using ClearBank.DeveloperTest.Config;
using ClearBank.DeveloperTest.Data;
using Microsoft.Extensions.Options;

namespace ClearBank.DeveloperTest.Services
{
    public class AccountDataStoreFactory : IAccountDataStoreFactory
    {
        private readonly DataStoreFactoryOptions _options;
        public AccountDataStoreFactory(IOptions<DataStoreFactoryOptions> options)
        {
            _options = options.Value;
        }

        public IAccountDataStore Create()
        {
            if (_options.DataStoreType.Equals(DataStoreType.Backup))
                return new BackupAccountDataStore();
            
            return new AccountDataStore();
        }
    }
}
