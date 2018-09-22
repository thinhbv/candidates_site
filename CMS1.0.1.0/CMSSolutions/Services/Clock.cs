using System;
using CMSSolutions.Caching;

namespace CMSSolutions.Services
{
    public class Clock : IClock
    {
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        public IVolatileToken When(TimeSpan duration)
        {
            return new AbsoluteExpirationToken(this, duration);
        }

        public IVolatileToken WhenUtc(DateTime absoluteUtc)
        {
            return new AbsoluteExpirationToken(this, absoluteUtc);
        }

        public class AbsoluteExpirationToken : IVolatileToken
        {
            private readonly IClock clock;
            private readonly DateTime invalidateUtc;

            public AbsoluteExpirationToken(IClock clock, DateTime invalidateUtc)
            {
                this.clock = clock;
                this.invalidateUtc = invalidateUtc;
            }

            public AbsoluteExpirationToken(IClock clock, TimeSpan duration)
            {
                this.clock = clock;
                invalidateUtc = this.clock.UtcNow.Add(duration);
            }

            public bool IsCurrent
            {
                get
                {
                    return clock.UtcNow < invalidateUtc;
                }
            }
        }
    }
}