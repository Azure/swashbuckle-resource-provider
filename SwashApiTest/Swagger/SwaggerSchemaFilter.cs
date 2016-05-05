using Swashbuckle.Swagger;
using System;
using System.Reflection;

namespace Swashbuckle.Swagger
{
    internal class SwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaRegistry schemaRegistry, Type type)
        {
            var name = type.FullName;

            if (type.IsGenericType)
            {
                // example of handling a generic resource type
                //if (type.GetGenericTypeDefinition() == typeof(Resource<>))
                //{
                //    schema.vendorExtensions.Add("x-ms-azure-resource", true);
                //}
            }

            UpdateEnumDefinitions(schema, type);

            // filter some types out of the swagger doc
            //if (!(name.StartsWith("SwashApiTest.") || type == typeof(object)))
            //{
            //    schema.vendorExtensions.Add(SwaggerConstants.ToRemoveVendorExtension, "");
            //}
        }

        /// <summary>
        /// Example of updating enum definitions, strip trailing "Internal" before generating swagger doc
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="type"></param>
        private void UpdateEnumDefinitions(Schema schema, Type type)
        {
            foreach (var property in schema.properties)
            {
                if (property.Value.@enum != null)
                {
                    var clrType = type.GetProperty(property.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).PropertyType;
                    property.Value.vendorExtensions.Add("x-ms-enum", new
                    {
                        name = clrType.Name.TrimEnd("Internal"),
                        modelAsString = bool.TrueString
                    });
                }
            }
        }
    }
}