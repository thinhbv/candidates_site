using CMSSolutions.Events;

namespace CMSSolutions.Web.Security.Permissions
{
    public interface IAuthorizationServiceEventHandler : IEventHandler
    {
        void Checking(CheckAccessContext context);

        void Adjust(CheckAccessContext context);

        void Complete(CheckAccessContext context);
    }
}