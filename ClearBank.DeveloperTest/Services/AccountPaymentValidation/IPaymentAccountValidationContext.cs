using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.AccountPaymentValidation
{
    public interface IPaymentAccountValidationContext
    {
        AccountPaymentValidationResult ValidateAccount(MakePaymentRequest paymentRequest, Account debtorAccount);
    }
}