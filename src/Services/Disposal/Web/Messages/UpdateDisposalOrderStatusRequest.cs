namespace Bazaar.Disposal.Web.Messages;

public readonly record struct UpdateDisposalOrderStatusRequest(
    DisposalStatus Status, string? CancelReason);
