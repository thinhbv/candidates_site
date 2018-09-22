using VortexSoft.MvcCornerStone.Data.Entity;

namespace VortexSoft.MvcCornerStone.Services.Payments
{
    public class PaymentsObjectContext : DbContextBase
    {
        public PaymentsObjectContext(string connectionString)
            : base(connectionString)
        {
        }
    }
}