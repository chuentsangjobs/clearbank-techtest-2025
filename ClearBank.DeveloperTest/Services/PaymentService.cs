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

            // TODO: Make result success should default to false - this will simply the code below from setting success = false for every failed senario
            result.Success = true;
            
            // TODO: Move out account validation into different strategy pattern
            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
   
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = false;
                    }
                    break;

                case PaymentScheme.FasterPayments:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                    {
                        result.Success = false;
                    }
                    else if (account.Balance < request.Amount)
                    {
                        result.Success = false;
                    }
                    break;

                case PaymentScheme.Chaps:
  
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                    {
                        result.Success = false;
                    }
                    else if (account.Status != AccountStatus.Live)
                    {
                        result.Success = false;
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
