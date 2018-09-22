using CMSSolutions.Events;
using CMSSolutions.Web.Security.Domain;

namespace CMSSolutions.Web.Security
{
    public interface IMembershipEventHandler : IEventHandler
    {
        void ValidateSucceeded(string username);

        void ValidateFailed(string username);

        void PasswordChanged(string username);

        void Registered(User user, LocalAccount localAccount, string password);

        void Confirmed(User user);

        void PasswordReset(User user, string passwordVerificationToken);
    }
}