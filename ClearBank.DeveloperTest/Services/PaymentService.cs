using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService(IAccountDataStoreFactory accountDataStoreFactory) : IPaymentService
    {

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var accountDataStore = accountDataStoreFactory.Create();

            var account = accountDataStore.GetAccount(request.DebtorAccountNumber);
            
            var result = new MakePaymentResult();

            if (account is null)
                return result;
            
            // TODO: Move out account validation into different strategy pattern
            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
   
                    if (account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = true;
                    }
                    break;

                case PaymentScheme.FasterPayments:
                    if (account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments)
                        && account.Balance >= request.Amount)
                    {
                        result.Success = true;
                    }
                    break;

                case PaymentScheme.Chaps:
  
                    if (account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps)
                        && account.Status == AccountStatus.Live)
                    {
                        result.Success = true;
                    }
                    break;
            }

            if (result.Success)
            {
                account.Balance -= request.Amount;
                accountDataStore.UpdateAccount(account);
            }

            // Do we need the reason for unsuccessful payment?
            return result;
        }
    }
}
