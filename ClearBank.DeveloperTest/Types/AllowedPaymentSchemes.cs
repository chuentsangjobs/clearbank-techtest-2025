namespace ClearBank.DeveloperTest.Types
{
    public enum AllowedPaymentSchemes
    {
        // TODO: Refactor to use numbers for readability
        FasterPayments = 1 << 0, // 1
        Bacs = 1 << 1, // 2
        Chaps = 1 << 2 // 4
    }
}
