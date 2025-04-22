using Newtonsoft.Json;

namespace ApiTests.Models;

public record DeleteProductResponse(
    [property: JsonProperty("status")] int Status);