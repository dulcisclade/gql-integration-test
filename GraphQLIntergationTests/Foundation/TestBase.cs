using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GraphQLIntegrationTests.Foundation;

public abstract class TestBase
{
    protected AutoFixture.Fixture Fixture { get; }
    protected DbContext DbContext { get; }
    protected HttpClient Client { get; }

    public TestBase()
    {
        DbContext = TestInitHelper.CreateDbContext();
        Fixture = TestInitHelper.CreteFixture();
    }

    protected StringContent CreateRequest(string query, object variables)
    {
        var param = new JObject {["query"] = query};

        var serializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters =
            {
                new GqlEnumConverter(),
                new IsoDateTimeConverter() {DateTimeFormat = "yyyy-MM-dd"}
            }
        };
        param["variables"] = JObject.FromObject(variables, serializer);
        return new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
    }
    
    protected (T Data, IReadOnlyCollection<GraphQLError> Errors) GetResult<T>(string resultJson, string path)
    {
        T data = default;

        var rawResultJsonObject = JObject.Parse(resultJson);
        var errorsJsonArray = rawResultJsonObject["errors"];
        var error = errorsJsonArray?.ToObject<IReadOnlyCollection<GraphQLError>>();

        if (error == null)
        {
            var dataJsonObject = rawResultJsonObject["data"];
            data = dataJsonObject == null ? default : dataJsonObject.SelectToken(path).ToObject<T>();
        }

        return (data, error);
    }
}