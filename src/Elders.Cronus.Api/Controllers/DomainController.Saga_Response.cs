﻿using System.Collections.Generic;

namespace Elders.Cronus.Api.Controllers
{
    public partial class DomainController
    {
        public class Saga_Response : BaseDomainModel_Response
        {
            public IEnumerable<Event_Response> Events { get; set; }
        }
    }
}
