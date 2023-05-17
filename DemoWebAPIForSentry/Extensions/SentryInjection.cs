using DemoWebAPIForSentry.Exceptions;
using Sentry;
using System.Security.Claims;
using System.Text.Json;

namespace DemoWebAPIForSentry.Extensions
{
    public static class SentryInjection
    {
        public static WebApplicationBuilder AddSentryService(this WebApplicationBuilder builder, IHttpContextAccessor httpContextAccessor)
        {

            builder.Logging.AddSentry(o =>
            {
                #region CORE CONFIGURATION

                o.Dsn = builder.Configuration["Sentry:Dsn"];

                #endregion

                #region ADDITIONAL CONFIGURATIONS
                //o.AddExceptionFilterForType<ArgumentNullException>(); // Ignore particular exception types
                o.AttachStacktrace = false;
                o.TracesSampleRate = 1; // Captures Transactions to observe latency and hit (eq: 0.25 = captures 25% of transactions
                o.SampleRate = 1;

                //FINGERPRINT (GROUPING) + USER 
                o.BeforeSend = (ev) =>
                {
                    if (ev.Exception is Api08Exception ex)
                    {

                        ev.User = new User
                        {
                            Username = "defaultUser",
                            Email = "admin@company.com",
                            IpAddress = httpContextAccessor.HttpContext!.Connection.RemoteIpAddress!.ToString()
                        };
                        ev.TransactionName = "COMPANYTransaciton_Api08_01";
                    }

                    if (ev.Exception is ScopeException ex2)
                    {
                        ev.SetFingerprint(new[] { "ScopeExceptionGROUP" });
                        ev.SetExtra("ExtraKey", "Extra Value");
                    }

                    return ev;
                };
                #endregion
            });

            return builder;
        }
    }
}
