namespace ClearBank.DeveloperTest.Types
{
    public enum AccountPaymentValidationResult
    {
        Success,
        AccountDoesNotSupportPaymentScheme,
        NotEnoughFunds,
        AccountNotActive
    }
}
