using GraphQL;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GraphQLIntegrationTests.Foundation;

public class GqlEnumConverter : StringEnumConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        Enum e = (Enum) value;
        var input = e.ToString();
        writer.WriteValue(input.ToConstantCase());
    }
}