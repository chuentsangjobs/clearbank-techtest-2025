using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.AccountPaymentValidation
{
    public interface IAccountValidationStrategy
    {
        PaymentScheme PaymentScheme { get; }
        AccountPaymentValidationResult Validate(MakePaymentRequest paymentRequest, Account debtorAccount);
    }
}