namespace WebSellerUI.DTOs;

public record OrderConfirmation(
    bool Confirmed,
    string? CancelReason
);
