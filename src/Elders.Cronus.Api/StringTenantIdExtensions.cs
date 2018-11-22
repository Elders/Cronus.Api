namespace Elders.Cronus.Api
{
    public static class StringTenantIdExtensions
    {
        static IUrnFormatProvider urnFormatProvider = new UberUrnFormatProvider();

        public static StringTenantId ToStringTenantId(this string input)
        {
            var urn = StringTenantUrn.Parse(input, urnFormatProvider);
            StringTenantId id = new StringTenantId(urn, urn.ArName);

            return id;
        }
    }
}
