namespace Bazaar.ScheduledTasks.Web.Messages;

public record struct LotsUbpdDisposalRequest(
    bool DisposeLotsUnfulfillableBeyondPolicyDuration);
