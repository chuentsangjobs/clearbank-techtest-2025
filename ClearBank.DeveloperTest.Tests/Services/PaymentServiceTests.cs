using AutoFixture.Xunit2;
using ClearBank.DeveloperTest.Config;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Services.AccountPaymentValidation;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class PaymentServiceTests
    {
        private readonly IAccountDataStoreFactory _accountDataStoreFactory;
        private readonly IAccountDataStore _accountDataStore;
        private readonly IPaymentService _sut;

        public PaymentServiceTests()
        {
            var options = new DataStoreFactoryOptions { DataStoreType = DataStoreType.Main };
            _accountDataStoreFactory = Substitute.For<IAccountDataStoreFactory>();
            _accountDataStore = Substitute.For<IAccountDataStore>();
            var validationStrategies = new IAccountValidationStrategy[] 
            { 
                new BacsValidationStrategy(), 
                new FasterPaymentsValidationStrategy(), 
                new ChapsValidationStrategy() 
            };
            var accountValidationContext = new PaymentAccountValidationContext(validationStrategies);
            _sut = new PaymentService(_accountDataStoreFactory, accountValidationContext);
            // TODO: move out payment validation scheme tests into separate files under each strategy
        }

        [Theory]
        [AutoData]
        public void MakePayment_ShouldBeUnsuccessful_WhenAccountNotFound(AllowedPaymentSchemes allowedPaymentScheme, 
            MakePaymentRequest request, Account account)
        {
            // Arrange
            request.PaymentScheme = PaymentScheme.Bacs;
            account.AllowedPaymentSchemes = allowedPaymentScheme;
            var expectedAccountBalance = account.Balance;
            _accountDataStoreFactory.Get()
                .Returns(_accountDataStore);
            _accountDataStore.GetAccount(request.DebtorAccountNumber)
                .ReturnsNull();

            // Act
            var result = _sut.MakePayment(request);

            // Assert
            result.Success.Should().BeFalse();
            account.Balance.Should().BeOneOf(expectedAccountBalance);
            _accountDataStore.Received().GetAccount(request.DebtorAccountNumber);
            _accountDataStore.DidNotReceive().UpdateAccount(account);
        }

        [Theory]
        [AutoData]
        public void MakePayment_ShouldBeSuccessful_WhenBacsPaymentRequested_AndAccountAllowsBacsPayments(
            MakePaymentRequest request, Account account)
        {
            // Arrange
            request.PaymentScheme = PaymentScheme.Bacs;
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs;
            var expectedAccountBalance = account.Balance - request.Amount;
            _accountDataStoreFactory.Get()
                .Returns(_accountDataStore);
            _accountDataStore.GetAccount(request.DebtorAccountNumber)
                .Returns(account);

            // Act
           var result = _sut.MakePayment(request);

            // Assert
            result.Success.Should().BeTrue();
            account.Balance.Should().BeOneOf(expectedAccountBalance);
            _accountDataStore.Received(1).UpdateAccount(account);
        }

        [Theory]
        [InlineAutoData(AllowedPaymentSchemes.FasterPayments)]
        [InlineAutoData(AllowedPaymentSchemes.Chaps)]
        public void MakePayment_ShouldBeUnsuccessful_WhenBacsPaymentRequested_AndAccountDoesNotAllowBacsPayments(
            AllowedPaymentSchemes allowedPaymentScheme, MakePaymentRequest request, Account account)
        {
            // Arrange
            request.PaymentScheme = PaymentScheme.Bacs;
            account.AllowedPaymentSchemes = allowedPaymentScheme;
            var expectedAccountBalance = account.Balance;
            _accountDataStoreFactory.Get()
                .Returns(_accountDataStore);
            _accountDataStore.GetAccount(request.DebtorAccountNumber)
                .Returns(account);

            // Act
            var result = _sut.MakePayment(request);

            // Assert
            result.Success.Should().BeFalse();
            account.Balance.Should().BeOneOf(expectedAccountBalance);
            _accountDataStore.DidNotReceive().UpdateAccount(account);
        }

        [Theory]
        [AutoData]
        public void MakePayment_ShouldBeSuccessful_WhenFasterPaymentRequested_AndAccountAllowsFasterPayments_WithEnoughBalance(
            MakePaymentRequest request, Account account)
        {
            // Arrange
            request.PaymentScheme = PaymentScheme.FasterPayments;
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;
            account.Balance = request.Amount + 10; // Ensure account has enough money
            var expectedAccountBalance = account.Balance - request.Amount;
            _accountDataStoreFactory.Get()
                .Returns(_accountDataStore);
            _accountDataStore.GetAccount(request.DebtorAccountNumber)
                .Returns(account);

            // Act
            var result = _sut.MakePayment(request);

            // Assert
            result.Success.Should().BeTrue();
            account.Balance.Should().BeOneOf(expectedAccountBalance);
            _accountDataStore.Received(1).UpdateAccount(account);
        }

        [Theory]
        [AutoData]
        public void MakePayment_ShouldBeUnsuccessful_WhenFasterPaymentRequested_AndAccountAllowsFasterPayments_ButNotEnoughBalance(
            MakePaymentRequest request, Account account)
        {
            // Arrange
            request.PaymentScheme = PaymentScheme.FasterPayments;
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;
            account.Balance = request.Amount - 1; // Ensure account does not have enough money
            var expectedAccountBalance = account.Balance;
            _accountDataStoreFactory.Get()
                .Returns(_accountDataStore);
            _accountDataStore.GetAccount(request.DebtorAccountNumber)
                .Returns(account);

            // Act
            var result = _sut.MakePayment(request);

            // Assert
            result.Success.Should().BeFalse();
            account.Balance.Should().BeOneOf(expectedAccountBalance);
            _accountDataStore.DidNotReceive().UpdateAccount(account);
        }

        [Theory]
        [InlineAutoData(AllowedPaymentSchemes.Bacs)]
        [InlineAutoData(AllowedPaymentSchemes.Chaps)]
        public void MakePayment_ShouldBeUnsuccessful_WhenFasterPaymentRequested_AndAccountDoesNotAllowFasterPayments(
            AllowedPaymentSchemes allowedPaymentScheme, MakePaymentRequest request, Account account)
        {
            // Arrange
            request.PaymentScheme = PaymentScheme.FasterPayments;
            account.AllowedPaymentSchemes = allowedPaymentScheme;
            account.Balance = request.Amount + 10; // Ensure account has enough money
            var expectedAccountBalance = account.Balance;
            _accountDataStoreFactory.Get()
                .Returns(_accountDataStore);
            _accountDataStore.GetAccount(request.DebtorAccountNumber)
                .Returns(account);

            // Act
            var result = _sut.MakePayment(request);

            // Assert
            result.Success.Should().BeFalse();
            account.Balance.Should().BeOneOf(expectedAccountBalance);
            _accountDataStore.DidNotReceive().UpdateAccount(account);
        }

        [Theory]
        [AutoData]
        public void MakePayment_ShouldBeSuccessful_WhenChapsPaymentRequested_AndAccountAllowsChapsPayments_AndIsLive(
            MakePaymentRequest request, Account account)
        {
            // Arrange
            request.PaymentScheme = PaymentScheme.Chaps;
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            account.Status = AccountStatus.Live;
            var expectedAccountBalance = account.Balance - request.Amount;
            _accountDataStoreFactory.Get()
                .Returns(_accountDataStore);
            _accountDataStore.GetAccount(request.DebtorAccountNumber)
                .Returns(account);

            // Act
            var result = _sut.MakePayment(request);

            // Assert
            result.Success.Should().BeTrue();
            account.Balance.Should().BeOneOf(expectedAccountBalance);
            _accountDataStore.Received(1).UpdateAccount(account);
        }

        [Theory]
        [InlineAutoData(AccountStatus.Disabled)]
        [InlineAutoData(AccountStatus.InboundPaymentsOnly)]
        public void MakePayment_ShouldBeUnsuccessful_WhenChapsPaymentRequested_AndAccountAllowsChapsPayments_AndIsNotLive(
            AccountStatus accountStatus, MakePaymentRequest request, Account account)
        {
            // Arrange
            request.PaymentScheme = PaymentScheme.Chaps;
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            account.Status = accountStatus;
            var expectedAccountBalance = account.Balance;
            _accountDataStoreFactory.Get()
                .Returns(_accountDataStore);
            _accountDataStore.GetAccount(request.DebtorAccountNumber)
                .Returns(account);

            // Act
            var result = _sut.MakePayment(request);

            // Assert
            result.Success.Should().BeFalse();
            account.Balance.Should().BeOneOf(expectedAccountBalance);
            _accountDataStore.DidNotReceive().UpdateAccount(account);
        }
    }
}
