using Quartz;

namespace MiataBot;

public sealed class TimedRoleExpiryJob(MiataDbContext db) : IJob
{
    public async ValueTask Execute(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}