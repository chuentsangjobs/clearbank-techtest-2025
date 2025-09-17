using AutoFixture.Xunit2;
using ClearBank.DeveloperTest.Services.AccountPaymentValidation;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services.AccountPaymentValidation
{
    public class FasterPaymentsValidationStrategyTests
    {
        private readonly FasterPaymentsValidationStrategy _sut;

        public FasterPaymentsValidationStrategyTests()
        {
            _sut = new FasterPaymentsValidationStrategy();
        }

        [Theory]
        [InlineAutoData(AllowedPaymentSchemes.Bacs)]
        [InlineAutoData(AllowedPaymentSchemes.Chaps)]

        public void Validate_ShouldReturnAccountDoesNotSupportPaymentScheme_WhenAccountDoesNotAllowsFasterPaymentScheme(AllowedPaymentSchemes paymentScheme, MakePaymentRequest request, Account account)
        {
            // Arrange
            account.AllowedPaymentSchemes = paymentScheme;

            // Act
            var result = _sut.Validate(request, account);

            // Assert
            result.Should().Be(AccountPaymentValidationResult.AccountDoesNotSupportPaymentScheme);
        }

        [Theory]
        [AutoData]

        public void Validate_ShouldReturnNotEnoughFunds_WhenAccountDoesNotHaveEnoughBalance(MakePaymentRequest request, Account account)
        {
            // Arrange
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;
            account.Balance = request.Amount - 1;

            // Act
            var result = _sut.Validate(request, account);

            // Assert
            result.Should().Be(AccountPaymentValidationResult.NotEnoughFunds);
        }

        [Theory]
        [AutoData]

        public void Validate_ShouldReturnSuccess_WhenAccountHasEnoughFundToMakeThePayment(MakePaymentRequest request, Account account)
        {
            // Arrange
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;
            account.Balance = request.Amount + 100;

            // Act
            var result = _sut.Validate(request, account);

            // Assert
            result.Should().Be(AccountPaymentValidationResult.Success);
        }
    }
}
