using CMSSolutions.Events;

namespace CMSSolutions.ContentManagement.Newsletters
{
    public interface INewsletterEventHandler : IEventHandler
    {
        void Subscribed(string emailAddress);

        void Unsubscribed(string emailAddress);
    }
}