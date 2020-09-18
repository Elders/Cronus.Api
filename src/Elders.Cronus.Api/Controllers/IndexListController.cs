using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Elders.Cronus.MessageProcessing;
using Elders.Cronus.EventStore.Index;
using Elders.Cronus.EventStore.Index.Handlers;

namespace Elders.Cronus.Api.Controllers
{
    [Route("Indices")]
    public class IndexListController : ApiControllerBase
    {
        private readonly ProjectionExplorer _projectionExplorer;
        private readonly CronusContext context;
        private readonly ProjectionHasher projectionHasher;
        private readonly TypeContainer<IEventStoreIndex> indicesMeta;

        public IndexListController(ProjectionExplorer projectionExplorer, CronusContext context, ProjectionHasher projectionHasher, TypeContainer<IEventStoreIndex> indicesMeta)
        {
            if (projectionExplorer is null) throw new ArgumentNullException(nameof(projectionExplorer));
            if (context is null) throw new ArgumentNullException(nameof(context));

            _projectionExplorer = projectionExplorer;
            this.context = context;
            this.projectionHasher = projectionHasher;
            this.indicesMeta = indicesMeta;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            IndexListDto result = new IndexListDto();
            foreach (var meta in indicesMeta.Items)
            {
                var id = new EventStoreIndexManagerId(meta.GetContractId(), context.Tenant);
                var dto = await _projectionExplorer.ExploreAsync(id, typeof(EventStoreIndexStatus));
                EventStoreIndexStatusState state = dto?.State as EventStoreIndexStatusState;
                var metaIndex = new IndexMeta()
                {
                    IndexContractId = meta.GetContractId(),
                    IndexName = meta.Name,
                };
                if (ReferenceEquals(null, state))
                {
                    metaIndex.Status = IndexStatus.NotPresent;
                }
                else
                {
                    metaIndex.Status = state.Status;
                }
                result.Indices.Add(metaIndex);
            }

            return new OkObjectResult(new ResponseResult<IndexListDto>(result));
        }
    }

    public class IndexListDto
    {
        public IndexListDto()
        {
            Indices = new List<IndexMeta>();
        }

        public List<IndexMeta> Indices { get; set; }
    }

    public class IndexMeta
    {
        public string IndexContractId { get; set; }

        public string IndexName { get; set; }

        public string Status { get; set; }
    }
}
