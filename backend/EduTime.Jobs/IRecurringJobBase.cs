using Hangfire.Server;
using System.Threading.Tasks;

namespace EduTime.Jobs
{
    public interface IRecurringJobBase
    {
        Task Execute(PerformContext context);
        void Register(string cronExpression);
    }
}
