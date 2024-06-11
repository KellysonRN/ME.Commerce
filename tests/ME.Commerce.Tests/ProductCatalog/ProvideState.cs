using System.Collections.Generic;

namespace ME.Commerce.Tests.ProductCatalog
{
    public record ProviderState(string State, IDictionary<string, object> Params);
}
