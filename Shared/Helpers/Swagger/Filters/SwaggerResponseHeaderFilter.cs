using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoList.Api.Shared.Helpers.Swagger.Attributes;

namespace TodoList.Api.Shared.Helpers.Swagger.Filters;

public class SwaggerResponseHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionAttributes = context
            .MethodInfo.GetCustomAttributes(typeof(SwaggerResponseHeaderAttribute), inherit: true)
            .Cast<SwaggerResponseHeaderAttribute>();

        foreach (var attr in actionAttributes)
        {
            operation.Responses!.TryGetValue("200", out IOpenApiResponse? response);

            if (response == null)
            {
                response = new OpenApiResponse { Description = "Success" };
                operation.Responses.Add("200", response);
            }

            if (response.Headers == null)
            {
                if (response is OpenApiResponse concreteResponse)
                {
                    concreteResponse.Headers = new Dictionary<string, IOpenApiHeader>();
                }
            }

            response.Headers?.Add(
                attr.HeaderName,
                new OpenApiHeader
                {
                    Description = attr.Description,
                    Schema = new OpenApiSchema { Type = JsonSchemaType.String },
                    Example = string.IsNullOrEmpty(attr.Example) ? null : System.Text.Json.Nodes.JsonValue.Create(attr.Example)
                }
            );
        }
    }
}
