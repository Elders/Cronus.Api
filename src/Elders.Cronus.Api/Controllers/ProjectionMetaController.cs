﻿using Elders.Cronus.Discoveries;
using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Elders.Cronus.MessageProcessing;
using System.ComponentModel.DataAnnotations;

namespace Elders.Cronus.Api.Controllers
{
    [Route("Projection")]
    public class ProjectionMetaController : ApiControllerBase
    {
        private readonly ProjectionExplorer _projectionExplorer;
        private readonly CronusContext context;
        private readonly ProjectionHasher projectionHasher;

        public ProjectionMetaController(ProjectionExplorer projectionExplorer, CronusContext context, ProjectionHasher projectionHasher)
        {
            if (projectionExplorer is null) throw new ArgumentNullException(nameof(projectionExplorer));

            _projectionExplorer = projectionExplorer;
            this.context = context;
            this.projectionHasher = projectionHasher;
        }

        [HttpGet, Route("Meta")]
        public async Task<IActionResult> Meta([FromQuery] RequestModel model)
        {
            IEnumerable<Assembly> loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.IsDynamic == false);
            Type metadata = loadedAssemblies
                .SelectMany(assembly => assembly.GetLoadableTypes()
                .Where(x => typeof(IProjection).IsAssignableFrom(x))
                .Where(x => x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0))
                .Where(x => x.GetContractId() == model.ProjectionContractId)
                .FirstOrDefault();

            if (metadata is null) return new BadRequestObjectResult(new ResponseResult<string>($"Projection with contract '{model.ProjectionContractId}' not found"));

            var id = new ProjectionVersionManagerId(model.ProjectionContractId, context.Tenant);
            ProjectionExplorer.ProjectionDto dto = await _projectionExplorer.ExploreAsync(id, typeof(ProjectionVersionsHandler));
            var state = dto?.State as ProjectionVersionsHandlerState;

            var metaProjection = new ProjectionMeta()
            {
                ProjectionContractId = metadata.GetContractId(),
                ProjectionName = metadata.Name,
                IsReplayable = typeof(IAmEventSourcedProjection).IsAssignableFrom(metadata)
            };

            if (state is null)
            {
                metaProjection.Versions.Add(new ProjectionVersion()
                {
                    Status = ProjectionStatus.NotPresent,
                    Hash = projectionHasher.CalculateHash(typeof(ProjectionVersionsHandler)),
                    Revision = 0
                });
            }
            else
            {
                foreach (var ver in state.AllVersions)
                {
                    metaProjection.Versions.Add(new ProjectionVersion()
                    {
                        Hash = ver.Hash,
                        Revision = ver.Revision,
                        Status = ver.Status
                    });
                }
            }

            return Ok(new ResponseResult<ProjectionMeta>(metaProjection));
        }

        public class RequestModel
        {
            [Required]
            public string ProjectionContractId { get; set; }
        }
    }
}
