namespace GraphQLIntegrationTests.Foundation;

public class GraphQLError
{
    public string Message { get; set; }
    public Extension Extension { get; set; }
}

public class Extension
{
    public string Code { get; set; }
    public string[] Codes { get; set; }
}