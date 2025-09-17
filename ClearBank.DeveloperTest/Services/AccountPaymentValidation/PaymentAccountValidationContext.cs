using System;
using System.Collections.Generic;
using System.Linq;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.AccountPaymentValidation
{
    public class PaymentAccountValidationContext(IEnumerable<IAccountValidationStrategy> accountValidationStrategies) : IPaymentAccountValidationContext
    {
        public AccountPaymentValidationResult ValidateAccount(MakePaymentRequest paymentRequest, Account debtorAccount)
        {
            var paymentSchemeStrategy = accountValidationStrategies.FirstOrDefault(x => x.PaymentScheme == paymentRequest.PaymentScheme)
                ?? throw new ApplicationException($"No account validation strategy configured for [{paymentRequest.PaymentScheme}]");

            var result = paymentSchemeStrategy.Validate(paymentRequest, debtorAccount);

            // TODO: log out metrics on validation results so we know how many have failed/succeeded
            return result;
        }
    }
}
