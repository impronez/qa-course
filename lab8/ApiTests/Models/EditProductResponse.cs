using Newtonsoft.Json;

namespace ApiTests.Models;

public record EditProductResponse(
    [property: JsonProperty("status")] int Status);