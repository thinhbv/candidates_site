using Autofac;

namespace CMSSolutions.Events
{
    public class EventsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterSource(new EventsRegistrationSource());
            base.Load(builder);
        }
    }
}