using System.Text.Json.Serialization;

namespace UITests.Configurations.AddingProductToCart;

public record AddingProductToCartData(
    [property: JsonPropertyName("product_link")] string ProductLink,
    [property: JsonPropertyName("quantity")] string Quantity,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("price")] string Price,
    [property: JsonPropertyName("total_price")] string TotalPrice,
    [property: JsonPropertyName("total_quantity")] string TotalQuantity);