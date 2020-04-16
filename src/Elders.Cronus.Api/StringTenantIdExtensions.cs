namespace Elders.Cronus.Api
{
    public static class StringTenantIdExtensions
    {
        static IUrnFormatProvider urnFormatProvider = new UberUrnFormatProvider();

        public static AggregateRootId ToStringTenantId(this string input)
        {
            var urn = AggregateUrn.Parse(input, urnFormatProvider);
            AggregateRootId id = new AggregateRootId(urn.AggregateRootName, urn);

            return id;
        }
    }
}
