namespace Wasl.Api.Extensions
{
    public class EmptyStringSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
                return;

            foreach (var property in schema.Properties)
            {
                if (property.Value.Type == "string" && property.Value.Example == null)
                {
                    property.Value.Example = new Microsoft.OpenApi.Any.OpenApiString("");
                }
            }
        }
    }
}