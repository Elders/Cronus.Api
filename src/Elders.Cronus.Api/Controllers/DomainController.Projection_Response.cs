﻿using System.Collections.Generic;

namespace Elders.Cronus.Api.Controllers
{
    public partial class DomainController
    {
        public class Projection_Response : BaseSerializableDomainModel_Response
        {
            public bool IsEventSourced { get; set; }

            public IEnumerable<Event_Response> Events { get; set; }
        }
    }
}
