using DemoWebAPIForSentry.Exceptions;
using DemoWebAPIForSentry.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using System.Threading;
using System.Transactions;

namespace DemoWebAPIForSentry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        [HttpGet]
        [Route("/api01")]
        public ActionResult Api01()
        {
            // Catch regular excaption without try-catch & SentrySdk.CaptureException()

            throw new Api01Exception();

        }

        [HttpGet]
        [Route("/api02")]
        public ActionResult Api02()
        {
            // Catch regular excaption WITH try-catch & SentrySdk.CaptureException()

            try
            {
                throw new Api02Exception();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/api03")]
        public ActionResult Api03()
        {
            // SCOPE Modification

            /* When an event is captured and sent to Sentry, SDKs will merge that event data with extra information from the current scope. SDKs will typically automatically manage the scopes for you in the framework integrations and you don't need to think about them. However, you should know what a scope is and how you can use it for your advantage.*/

            try
            {
                SentrySdk.ConfigureScope(scope =>
                {
                    //TAG
                    scope.SetTag("COMPANY_MyCustomTag", "My Custom Tag Value");

                    scope.User = new User
                    {
                        Id = "0001",
                        Email = "jane.doe@example.com"
                    };

                    // Context
                    scope.Contexts["myContext"] = new
                    {
                        contextVar1 = "Context Value 1",
                        contextVar2 = "Context Value 2"
                    };

                    // Transciton Name
                    scope.TransactionName = "COMPANYTransactionName";

                    // Attachment

                    scope.AddAttachment("file2.txt");

                    // Level
                    scope.Level = SentryLevel.Error;
                });

                throw new ScopeException("Scope Example Error");
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return BadRequest();
            }


        }

        [HttpGet]
        [Route("/api04")]
        public ActionResult Api04()
        {
            // Adding BREADCRUMP 

            /* Sentry uses breadcrumbs to create a trail of events that happened prior to an issue. These events are very similar to traditional logs, but can record more rich structured data. */

            try
            {
                var user = HttpContext.User;
                SentrySdk.AddBreadcrumb(
                    message: $"Authenticated user {user.Identity.Name}",
                    category: "auth",
                    level: BreadcrumbLevel.Info);

                throw new BreadcrumpException("Breadcrump sample error");
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/api05")]
        public ActionResult Api05()
        {
            // Adding MESSAGE + Creating Alert Rule + Triggering Alert

            bool X() => true;
            SentrySdk.CaptureMessage($"Method {nameof(X)} executed", SentryLevel.Info);

            return Ok();
        }

        [HttpGet]
        [Route("/api06")]
        public ActionResult Api06()
        {
            // TRANSACTION + Create Latency + Create Latency Alert Rule + Trigger Alert
            // SPAN: A piece in TRANSACTION. Transaction itself is also a SPAN (Parent Span)
            // https://docs.sentry.io/product/performance/metrics/?original_referrer=https%3A%2F%2Fwww.google.com%2F#average-transaction-duration 

            var transaction = SentrySdk.StartTransaction("COMPANYTransaciton_Api06_02", "COMPANYOperation_Api06");
            SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

            var processSpan = transaction.StartChild(
                "process_Api06_02",
                "description_Api06_02"
            );

            Thread.Sleep(3000);

            processSpan.Finish();

            transaction.Finish();

            return Ok();
        }

        [HttpGet]
        [Route("/api07")]
        public ActionResult Api07()
        {
            // Create particular TRANSACTION + Create THROUGHPUT Alert if more than 3 Requests come in 1 Minute interval

            var transaction = SentrySdk.StartTransaction("COMPANYTransaciton_Api07_01", "COMPANYOperation_Api07");
            SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

            var processSpan = transaction.StartChild(
                "process_Api07_01",
                "description_Api07_01"
            );

            // Do Something

            processSpan.Finish();

            transaction.Finish();

            return Ok();
        }

        [HttpGet]
        [Route("/api08")]
        public ActionResult Api08()
        {
            // Set a Particular Transaction Name for a Particular Exception Type
            throw new Api08Exception();
            return Ok();
        }

        [TransactionalSentry]
        [HttpGet]
        [Route("/api09")]
        public ActionResult Api09()
        {
            // The [TransactionalSentry] Attibute implements a Filter (Transactional pattern) and creates a custom Sentry Transaction with the Action's name and finishes  the transaction when actions finishes
            Thread.Sleep(1000);
            //throw new Api09Exception();
            return Ok();
        }

    }
}

