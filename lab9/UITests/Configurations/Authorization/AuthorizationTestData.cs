using System.Text.Json.Serialization;

namespace UITests.Configurations.Authorization;

public record AuthorizationTestData(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("expectedMessage")] string ExpectedMessage);