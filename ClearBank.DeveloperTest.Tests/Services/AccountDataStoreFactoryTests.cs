using System;
using ClearBank.DeveloperTest.Config;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class AccountDataStoreFactoryTests
    {
        private readonly IOptions<DataStoreFactoryOptions> _options;
        private readonly AccountDataStoreFactory _sut;

        public AccountDataStoreFactoryTests()
        {
            _options = Substitute.For<IOptions<DataStoreFactoryOptions>>();
            _sut = new AccountDataStoreFactory(_options, [new AccountDataStore(), new BackupAccountDataStore()]);
        }

        [Fact]
        public void Get_ShouldReturnBackUpDataStore_WhenOptionIsConfiguredForBackUp()
        {
            // Arrange
            _options.Value.Returns(new DataStoreFactoryOptions { DataStoreType = DataStoreType.Backup });

            // Act
            var actualDatStore = _sut.Get();
            
            // Assert
            actualDatStore.Should().BeOfType<BackupAccountDataStore>();
        }

        [Fact]
        public void Get_ShouldReturnMainDataStore_WhenNoOptionIsConfigured()
        {
            // Arrange
            _options.Value.ReturnsNull();

            // Act
            var actualDatStore = _sut.Get();

            // Assert
            actualDatStore.Should().BeOfType<AccountDataStore>();
        }

        [Fact]
        public void Get_ShouldThrowException_WhenNoDataStoreIsFound()
        {
            // Arrange
            _options.Value.Returns(new DataStoreFactoryOptions { DataStoreType = DataStoreType.Main });
            var sut = new AccountDataStoreFactory(_options, []);

            // Act
            var act = () => sut.Get();

            // Assert
            act.Should().Throw<ApplicationException>().Which.Message.Should().Contain("Could not find main datastore");
        }
    }
}
