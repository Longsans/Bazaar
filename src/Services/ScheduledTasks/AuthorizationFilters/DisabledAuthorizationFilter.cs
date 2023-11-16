using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Bazaar.ScheduledTasks.AuthorizationFilters;

public class DisabledAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}
