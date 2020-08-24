using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elders.Cronus.Dashboard.Services
{
    public class CronusClient
    {
        private readonly EventStoreExplorer eventExplorer;

        public CronusClient(EventStoreExplorer eventExplorer)
        {
            this.eventExplorer = eventExplorer;
        }

        public Task<AggregateDto> GetAggregate(string aggregateId)
        {
            if (string.IsNullOrEmpty(aggregateId)) throw new ArgumentNullException(nameof(aggregateId));

            return eventExplorer.ExploreAsync(aggregateId.ToStringTenantId());
        }
    }
}
