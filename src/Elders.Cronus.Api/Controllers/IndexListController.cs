﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly ICronusContextAccessor contextAccessor;
        private readonly TypeContainer<IEventStoreIndex> indicesMeta;

        public IndexListController(ProjectionExplorer projectionExplorer, ICronusContextAccessor contextAccessor, TypeContainer<IEventStoreIndex> indicesMeta)
        {
            if (projectionExplorer is null) throw new ArgumentNullException(nameof(projectionExplorer));
            if (contextAccessor is null) throw new ArgumentNullException(nameof(contextAccessor));

            _projectionExplorer = projectionExplorer;
            this.contextAccessor = contextAccessor;
            this.indicesMeta = indicesMeta;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            IndexListDto result = new IndexListDto();
            foreach (var meta in indicesMeta.Items)
            {
                var id = new EventStoreIndexManagerId(meta.GetContractId(), contextAccessor.CronusContext.Tenant);
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
