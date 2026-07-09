using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoList.Api.Common.Helpers.Swagger.Attributes;

namespace TodoList.Api.Common.Helpers.Swagger.Filters;

public class SwaggerRequestCookieFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var cookieAttributes = context
            .MethodInfo.GetCustomAttributes(typeof(SwaggerRequestCookieAttribute), inherit: true)
            .Cast<SwaggerRequestCookieAttribute>();

        foreach (var attr in cookieAttributes)
        {
            operation.Parameters = [];

            operation.Parameters.Add(
                new OpenApiParameter
                {
                    Name = attr.Name,
                    In = ParameterLocation.Cookie,
                    Required = attr.IsRequired,
                    Description = attr.Description,
                    Schema = new OpenApiSchema { Type = JsonSchemaType.String },
                }
            );
        }
    }
}
