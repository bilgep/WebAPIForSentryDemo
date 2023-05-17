using Microsoft.AspNetCore.Mvc.Filters;
using Sentry;
using System.Data.Common;
using System.Transactions;

namespace DemoWebAPIForSentry.Filters
{
    public class TransactionalSentryAttribute : Attribute, IFilterFactory
    {

        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new TransactionalSentryFilter();
        }

        private class TransactionalSentryFilter : IActionFilter
        {
            private TransactionScope _transactionScope = default!;
            private string _transactionName = "defaultTransactionName";
            private ITransaction _transaction = default!;

            public void OnActionExecuting(ActionExecutingContext context)
            {
                context.RouteData.Values.TryGetValue("action", out object? myValue);
                if(myValue != null) _transactionName = myValue.ToString() ?? default!;
                _transactionScope = new TransactionScope();
                Console.WriteLine($"Sentry Filter > TransactionalSentryFilter > OnActionExecuting > {DateTime.UtcNow.ToString()}");
                
                _transaction = SentrySdk.StartTransaction($"COMPANYTransaciton_{_transactionName}", $"COMPANYOperation_{_transactionName}");
                SentrySdk.ConfigureScope(scope => scope.Transaction = _transaction);

            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                Console.WriteLine($"Sentry Filter > TransactionalSentryFilter > OnActionExecuted > {DateTime.UtcNow.ToString()}");

                _transaction.Finish();

                if (context.Exception == null)
                    _transactionScope.Complete();
            }
        }
    }
}

