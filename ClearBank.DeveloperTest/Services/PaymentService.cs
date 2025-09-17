using ClearBank.DeveloperTest.Services.AccountPaymentValidation;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService(IAccountDataStoreFactory accountDataStoreFactory, IPaymentAccountValidationContext accountValidationContext) : IPaymentService
    {
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var accountDataStore = accountDataStoreFactory.Get();
            var account = accountDataStore.GetAccount(request.DebtorAccountNumber);
            
            var result = new MakePaymentResult();

            if (account is null)
                return result;

            var validationResult = accountValidationContext.ValidateAccount(request, account);

            if (validationResult.Equals(AccountPaymentValidationResult.Success))
            {
                account.Balance -= request.Amount;
                accountDataStore.UpdateAccount(account);
                result.Success = true;
            }

            return result;
        }
    }
}
