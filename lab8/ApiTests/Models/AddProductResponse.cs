using Newtonsoft.Json;

namespace ApiTests.Models;

public record AddProductResponse(
    [property: JsonProperty("status")] int Status,
    [property: JsonProperty("id")] int Id);