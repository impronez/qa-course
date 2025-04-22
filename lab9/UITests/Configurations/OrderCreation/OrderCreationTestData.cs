using System.Text.Json.Serialization;

namespace UITests.Configurations.OrderCreation;

public record OrderCreationTestData(
    [property: JsonPropertyName("product_link")] string ProductLink,
    [property: JsonPropertyName("quantity")] string Quantity,
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("customer_name")] string CustomerName,
    [property: JsonPropertyName("address")] string Address,
    [property: JsonPropertyName("note")] string Note,
    [property: JsonPropertyName("alert")] string Alert);