namespace Bazaar.ScheduledTasks.BackgroundJobs;

public interface IBackgroundJob
{
    Task ExecuteAsync();
}
