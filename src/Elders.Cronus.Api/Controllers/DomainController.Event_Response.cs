using System.Collections.Generic;

namespace Elders.Cronus.Api.Controllers
{
    public partial class DomainController
    {
        public class Event_Response : BaseSerializableDomainModel_Response
        {
            public Event_Response()
            {
                Properties = new List<string>();
            }

            public List<string> Properties { get; set; }
        }
    }
}
