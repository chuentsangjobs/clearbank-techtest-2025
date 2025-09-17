using AutoFixture.Xunit2;
using ClearBank.DeveloperTest.Services.AccountPaymentValidation;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services.AccountPaymentValidation
{
    public class BacsValidationStrategyTests
    {
        private readonly BacsValidationStrategy _sut;

        public BacsValidationStrategyTests()
        {
            _sut = new BacsValidationStrategy();
        }

        [Theory]
        [AutoData]
        public void Validate_ShouldReturnSuccess_WhenAccountAllowsBacsPaymentScheme(MakePaymentRequest request, Account account)
        {
            // Arrange
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs;

            // Act
            var result = _sut.Validate(request, account);

            // Assert
            result.Should().Be(AccountPaymentValidationResult.Success);
        }

        [Theory]
        [InlineAutoData(AllowedPaymentSchemes.FasterPayments)]
        [InlineAutoData(AllowedPaymentSchemes.Chaps)]

        public void Validate_ShouldReturnAccountDoesNotSupportPaymentScheme_WhenAccountDoesNotAllowsBacsPaymentScheme(AllowedPaymentSchemes paymentScheme, MakePaymentRequest request, Account account)
        {
            // Arrange
            account.AllowedPaymentSchemes = paymentScheme;

            // Act
            var result = _sut.Validate(request, account);

            // Assert
            result.Should().Be(AccountPaymentValidationResult.AccountDoesNotSupportPaymentScheme);
        }
    }
}
