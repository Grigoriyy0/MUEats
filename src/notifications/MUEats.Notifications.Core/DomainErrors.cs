using Primitives;

namespace MUEats.Notifications.Core;

public static class DomainErrors
{
    public static class Notification
    {
        public static readonly Error EventDataIsEmpty = GeneralError.ValueIsIncorrect("notification.body", "notification body cannot be empty.");

        public static readonly Error RecipientInfoIsEmpty = GeneralError.ValueIsIncorrect("notification.recipient_info", "recipient info cannot be empty.");
    }
}