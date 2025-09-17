using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.AccountPaymentValidation
{
    public class FasterPaymentsValidationStrategy : IAccountValidationStrategy
    {
        public PaymentScheme PaymentScheme => PaymentScheme.FasterPayments;

        public AccountPaymentValidationResult Validate(MakePaymentRequest paymentRequest, Account debtorAccount)
        {
            if (!debtorAccount.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                return AccountPaymentValidationResult.AccountDoesNotSupportPaymentScheme;

            if (debtorAccount.Balance < paymentRequest.Amount)
                return AccountPaymentValidationResult.NotEnoughFunds;

            return AccountPaymentValidationResult.Success;
        }
    }
}
