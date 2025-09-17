using AutoFixture.Xunit2;
using ClearBank.DeveloperTest.Services.AccountPaymentValidation;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services.AccountPaymentValidation
{
    public class ChapsValidationStrategyTests
    {
        private readonly ChapsValidationStrategy _sut;

        public ChapsValidationStrategyTests()
        {
            _sut = new ChapsValidationStrategy();
        }

        [Theory]
        [InlineAutoData(AllowedPaymentSchemes.FasterPayments)]
        [InlineAutoData(AllowedPaymentSchemes.Bacs)]

        public void Validate_ShouldReturnAccountDoesNotSupportPaymentScheme_WhenAccountDoesNotAllowsChapsPaymentScheme(AllowedPaymentSchemes paymentScheme, MakePaymentRequest request, Account account)
        {
            // Arrange
            account.AllowedPaymentSchemes = paymentScheme;

            // Act
            var result = _sut.Validate(request, account);

            // Assert
            result.Should().Be(AccountPaymentValidationResult.AccountDoesNotSupportPaymentScheme);
        }

        [Theory]
        [InlineAutoData(AccountStatus.Disabled)]
        [InlineAutoData(AccountStatus.InboundPaymentsOnly)]
        public void Validate_ShouldReturnSuccess_WhenAccountAllowsChapsPaymentScheme_AndAccountIsNotLive(AccountStatus accountStatus, MakePaymentRequest request, Account account)
        {
            // Arrange
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            account.Status = accountStatus;

            // Act
            var result = _sut.Validate(request, account);

            // Assert
            result.Should().Be(AccountPaymentValidationResult.AccountNotActive);
        }

        [Theory]
        [AutoData]
        public void Validate_ShouldReturnSuccess_WhenAccountAllowsChapsPaymentScheme_AndAccountIsLive(MakePaymentRequest request, Account account)
        {
            // Arrange
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            account.Status = AccountStatus.Live;

            // Act
            var result = _sut.Validate(request, account);

            // Assert
            result.Should().Be(AccountPaymentValidationResult.Success);
        }
    }
}
