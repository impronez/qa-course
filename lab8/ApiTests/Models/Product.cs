using Newtonsoft.Json;

namespace ApiTests.Models;

public record Product(
    [property: JsonProperty("id")] int Id,
    [property: JsonProperty("category_id")] int CategoryId,
    [property: JsonProperty("title")] string Title,
    [property: JsonProperty("alias")] string Alias,
    [property: JsonProperty("content")] string Content,
    [property: JsonProperty("price")] double Price,
    [property: JsonProperty("old_price")] double OldPrice,
    [property: JsonProperty("status")] int Status,
    [property: JsonProperty("keywords")] string Keywords,
    [property: JsonProperty("description")] string Description,
    [property: JsonProperty("hit")] int Hit);