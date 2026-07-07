namespace TodoList.Api.Common.Helpers.Swagger.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SwaggerRequestCookieAttribute(
    string name,
    string description = "",
    bool isRequired = false
) : Attribute
{
    public string Name { get; init; } = name;
    public string Description { get; init; } = description;
    public bool IsRequired { get; init; } = isRequired;
}
