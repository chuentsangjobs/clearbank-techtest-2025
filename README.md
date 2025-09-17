### Test Description
In the 'PaymentService.cs' file you will find a method for making a payment. At a high level the steps for making a payment are:

 - Lookup the account the payment is being made from
 - Check the account is in a valid state to make the payment
 - Deduct the payment amount from the account's balance and update the account in the database
 
What we’d like you to do is refactor the code with the following things in mind:  
 - Adherence to SOLID principals
 - Testability  
 - Readability 

We’d also like you to add some unit tests to the ClearBank.DeveloperTest.Tests project to show how you would test the code that you’ve produced. The only specific ‘rules’ are:  

 - The solution should build.
 - The tests should all pass.
 - You should not change the method signature of the MakePayment method.

You are free to use any frameworks/NuGet packages that you see fit.  
 
You should plan to spend around 1 to 3 hours to complete the exercise.

### Areas for refactor 
## PaymentService.cs
1. Let's check for null request, do we need to validate request values?
1. Abstract account data store creation into a Factory method so we a don't have to instantiate concrete data types. 
1. Move app settings into Options pattern - Encapsulation: setting related to the class & Separation of Concerns: don't have to worry where the configuration comes from - this will be wired up in start up.
1. We should validate if account can be found and exit early
1. Move out account validation into different strategy pattern - Single responsibilty + Open modification/Closed principle allowing for easy addition of validation schemes
1. Should we extend MakePaymentResult to contain an error code for unsuccessful payment?
1. Write some tests to capture the main logic of payment service before attempting refactor

## TODOs
Given more time I would have written unit tests for each validation strategy class whilst refactoring out tests in PaymentServiceTests.cs
