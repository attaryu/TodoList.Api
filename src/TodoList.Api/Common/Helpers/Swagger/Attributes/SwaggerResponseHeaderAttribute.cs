namespace TodoList.Api.Common.Helpers.Swagger.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SwaggerResponseHeaderAttribute(
    string headerName,
    string description,
    string example = ""
) : Attribute
{
    public string HeaderName { get; } = headerName;
    public string Description { get; } = description;
    public string Example { get; } = example;
}
