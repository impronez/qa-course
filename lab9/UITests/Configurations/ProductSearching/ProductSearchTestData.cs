using System.Text.Json.Serialization;

namespace UITests.Configurations.ProductSearching;

public record ProductSearchTestData(
    [property: JsonPropertyName("search_query")] string SearchQuery,
    [property: JsonPropertyName("quantity")] string Quantity);