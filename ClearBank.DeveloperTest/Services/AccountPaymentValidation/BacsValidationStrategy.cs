using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.AccountPaymentValidation
{
    public class BacsValidationStrategy : IAccountValidationStrategy
    {
        public PaymentScheme PaymentScheme => PaymentScheme.Bacs;

        public AccountPaymentValidationResult Validate(MakePaymentRequest paymentRequest, Account debtorAccount)
        {
            if (debtorAccount.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                return AccountPaymentValidationResult.Success;

            return AccountPaymentValidationResult.AccountDoesNotSupportPaymentScheme;
        }
    }
}
