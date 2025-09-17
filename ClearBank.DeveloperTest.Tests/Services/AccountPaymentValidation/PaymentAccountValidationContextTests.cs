using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using ClearBank.DeveloperTest.Services.AccountPaymentValidation;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services.AccountPaymentValidation
{
    public class PaymentAccountValidationContextTests
    {
        private readonly IAccountValidationStrategy _bacsValidationStrategy;
        private readonly IAccountValidationStrategy _chapsValidationStrategy;
        private readonly IAccountValidationStrategy _fasterPaymentsValidationStrategy;
        private List<IAccountValidationStrategy> _validationStrategies;
        readonly PaymentAccountValidationContext _sut;

        public PaymentAccountValidationContextTests()
        {
            _bacsValidationStrategy = Substitute.For<IAccountValidationStrategy>();
            _bacsValidationStrategy.PaymentScheme.Returns(PaymentScheme.Bacs);
            _fasterPaymentsValidationStrategy = Substitute.For<IAccountValidationStrategy>();
            _fasterPaymentsValidationStrategy.PaymentScheme.Returns(PaymentScheme.FasterPayments);
            _chapsValidationStrategy = Substitute.For<IAccountValidationStrategy>();
            _chapsValidationStrategy.PaymentScheme.Returns(PaymentScheme.Chaps);
            _validationStrategies = [
                _bacsValidationStrategy,
                _fasterPaymentsValidationStrategy,
                _chapsValidationStrategy ];
            _sut = new PaymentAccountValidationContext(_validationStrategies);
        }

        [Theory]
        [InlineAutoData(PaymentScheme.Bacs)]
        [InlineAutoData(PaymentScheme.FasterPayments)]
        [InlineAutoData(PaymentScheme.Chaps)]
        public void ValidateAccount_ShouldSelectTheCorrectPaymentStrategy(PaymentScheme paymentScheme, MakePaymentRequest request, Account account)
        {
            // Arrange
            request.PaymentScheme = paymentScheme;
            var expectedStrategy = _validationStrategies.First(x => x.PaymentScheme == paymentScheme);
            var expectedResult = AccountPaymentValidationResult.AccountDoesNotSupportPaymentScheme;
            expectedStrategy.Validate(request, account).Returns(expectedResult);

            // Act
            var result = _sut.ValidateAccount(request, account);

            // Assert
            expectedStrategy.Received(1).Validate(request, account);
            result.Should().Be(expectedResult); 
        }

        [Theory]
        [InlineAutoData(PaymentScheme.Bacs)]
        [InlineAutoData(PaymentScheme.FasterPayments)]
        [InlineAutoData(PaymentScheme.Chaps)]
        public void ValidateAccount_ShouldThrowExpcetion_WhenUnableToFindValidationStrategy(PaymentScheme paymentScheme, MakePaymentRequest request, Account account)
        {
            // Arrange
            request.PaymentScheme = paymentScheme;
            var expectedStrategy = _validationStrategies.First(x => x.PaymentScheme == paymentScheme);
            _validationStrategies.Remove(expectedStrategy);

            // Act
            var act = () => _sut.ValidateAccount(request, account);

            // Assert
            act.Should().ThrowExactly<ApplicationException>()
                .WithMessage($"No account validation strategy configured for [{paymentScheme}]");
        }
    }
}
