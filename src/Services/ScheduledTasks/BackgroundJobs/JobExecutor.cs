namespace Bazaar.ScheduledTasks.BackgroundJobs;

public static class JobExecutor
{
    public static async Task GetAndExecute<T>(IServiceProvider sp)
        where T : IBackgroundJob
    {
        var job = sp.GetRequiredService<T>();
        await job.ExecuteAsync();
    }
}
