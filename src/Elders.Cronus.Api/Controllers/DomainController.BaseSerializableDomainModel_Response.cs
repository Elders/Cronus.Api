namespace Elders.Cronus.Api.Controllers
{
    public partial class DomainController
    {
        public class BaseSerializableDomainModel_Response : BaseDomainModel_Response
        {
            /// <summary>
            /// Data contract name
            /// </summary>
            public string Id { get; set; }
        }
    }
}
