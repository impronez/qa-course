using System.Text.Json;

namespace UITests.Utilities;

public static class JsonDataConverter
{
    public static IEnumerable<object[]> Get<T>(string filePath, Func<T, object[]> selector)
    {
        var jsonText = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var testData = JsonSerializer.Deserialize<List<T>>(jsonText, options)
                       ?? throw new InvalidOperationException("Не удалось десериализовать JSON");

        foreach (var item in testData)
        {
            yield return selector(item);
        }
    }

}