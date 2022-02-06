using DigitalSkynet.DotnetCore.DataStructures.Exceptions.Api;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EduTime.Jobs
{
    public abstract class RecurringJobBase : IRecurringJobBase
    {
        protected readonly ILogger _logger;

        protected RecurringJobBase(ILogger logger)
        {
            _logger = logger;
        }

        // TODO: prevent concurrent execution
        // TODO: automatic retry
        public abstract Task Execute(PerformContext context);

        public void Register(string cronExpression)
        {
            RecurringJob.AddOrUpdate(() => Execute(null), cronExpression);
        }

        protected void Log(PerformContext context, string message, [CallerMemberName] string caller = "")
        {
            const string formattedMessage = "Recurring job '{0}.{1}' {2}";
            _logger.LogDebug(formattedMessage, GetType().Name, caller, message);
            context?.WriteLine(formattedMessage);
        }

        protected void Log(PerformContext context, Exception e, [CallerMemberName] string caller = "")
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Recurring job '{GetType().Name}.{caller}' failed with {e.GetType().Name}");
            sb.AppendLine($"Exception: {e.Message}");
            sb.AppendLine($"Trace: {e.StackTrace}");

            if(e is ApiException apiEx)
            {
                sb.AppendLine($"System message: {apiEx.SystemMessage}");
            }

            if(e.InnerException != null)
            {
                sb.AppendLine($"Inner exception: {e.InnerException.GetType().Name}: {e.InnerException.Message}");
            }

            var formattedMessage = sb.ToString();

            _logger.LogError(formattedMessage);
            context?.WriteLine(formattedMessage);
        }
    }
}
