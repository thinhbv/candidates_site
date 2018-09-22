using Autofac;

namespace VortexSoft.MvcCornerStone.Services.Payments
{
    public class PaymentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerLifetimeScope();
        }
    }
}