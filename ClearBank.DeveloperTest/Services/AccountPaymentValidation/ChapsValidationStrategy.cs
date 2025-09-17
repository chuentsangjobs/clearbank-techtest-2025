using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.AccountPaymentValidation
{
    public class ChapsValidationStrategy : IAccountValidationStrategy
    {
        public PaymentScheme PaymentScheme => PaymentScheme.Chaps;

        public AccountPaymentValidationResult Validate(MakePaymentRequest paymentRequest, Account debtorAccount)
        {
            if (!debtorAccount.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                return AccountPaymentValidationResult.AccountDoesNotSupportPaymentScheme;

            if (debtorAccount.Status != AccountStatus.Live)
                return AccountPaymentValidationResult.AccountNotActive;

            return AccountPaymentValidationResult.Success;
        }
    }
}
