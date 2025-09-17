using System;
using System.Collections.Generic;
using System.Linq;
using ClearBank.DeveloperTest.Config;
using ClearBank.DeveloperTest.Data;
using Microsoft.Extensions.Options;

namespace ClearBank.DeveloperTest.Services
{
    public class AccountDataStoreFactory(IOptions<DataStoreFactoryOptions> options, IEnumerable<IAccountDataStore> accountDataStores) : IAccountDataStoreFactory
    {
        public IAccountDataStore Get()
        {
            if (options.Value?.DataStoreType == DataStoreType.Backup)
               return accountDataStores.FirstOrDefault(x => x.Type == DataStoreType.Backup) 
                    ?? throw new ApplicationException("Could not find backup account datastore");

            return accountDataStores.FirstOrDefault(x => x.Type == DataStoreType.Main)
                 ?? throw new ApplicationException("Could not find main datastore");
        }
    }
}
