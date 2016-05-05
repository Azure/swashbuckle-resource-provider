using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace Swashbuckle.Swagger
{
    internal class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            // Add schema reference for IntelliSense
            //swaggerDoc.vendorExtensions.Add("$schema", "https://github.com/Azure/autorest/blob/master/schema/swagger-extensions.json");

            // By default, the Swagger document will be pointing to wherever it was generated (e.g. localhost).
            //swaggerDoc.host = new Uri("http://swashapitest.azurewebsites.net/").Host;

            RemoveNonResourceOperations(swaggerDoc);

            CreateGlobalParameters(swaggerDoc);

            // Remove filtered out types
            var defToRemove = (from i in swaggerDoc.definitions
                               where i.Value.vendorExtensions.ContainsKey(SwaggerConstants.ToRemoveVendorExtension)
                               select i).ToArray();

            foreach (var i in defToRemove)
            {
                swaggerDoc.definitions.Remove(i.Key);
            }

            // Update paths and responses
            foreach (var path in swaggerDoc.paths)
            {
                // Strip trailing "Async" from all operations
                foreach (var operationPair in path.Value.Operations())
                {
                    var operation = operationPair.Value;

                    operation.operationId = operation.operationId.TrimEnd("Async");

                    // add description to all operations. This can be read from a resource file
                    operation.description = operation.operationId;

                    // Add default response or update resposnes
                    Response resposne = new Response();
                    resposne.description = "default response. To add more details later";
                    operation.responses.Add("default", resposne);
                }
            }

            SortDocument(swaggerDoc);
        }

        /// <summary>
        /// Remove some controllers from the swagger doc
        /// </summary>
        /// <param name="swaggerDoc"></param>
        private static void RemoveNonResourceOperations(SwaggerDocument swaggerDoc)
        {
            var pathsToRemove = (
                from path in swaggerDoc.paths
                from operation in path.Value.Operations()
                where new[] {
                    //nameof(OperationController),
                    //nameof(OperationMetadataController),
                    //nameof(SubscriptionController),
                    //nameof(SubscriptionActionController),
                    "test"
                }.Contains(operation.Value.tags[0] + nameof(Controller))
                select path
                ).Distinct().ToArray();

            foreach (var path in pathsToRemove)
            {
                swaggerDoc.paths.Remove(path);
            }
        }

        /// <summary>
        /// Replaces the per-operation subscriptionId parameter with a reference
        /// to a global parameter. Add an apiVersion parameter to all operations.
        /// </summary>
        private static void CreateGlobalParameters(SwaggerDocument swaggerDoc)
        {
            swaggerDoc.parameters = new Dictionary<string, Parameter>
            {
                { "subscriptionId",
                    new Parameter
                      {
                        name = "subscriptionId",
                        @in = "path",
                        required = true,
                        type = "string",
                        description = "The Subscription ID."
                       }
                },
                { "api-version",
                    new Parameter
                      {
                        name = "api-version",
                        @in = "query",
                        required = true,
                        type = "string",
                        description = "Client Api Version."
                      }
                },
            };

            foreach (var path in swaggerDoc.paths)
            {
                foreach (var operation in path.Value.Operations().Select(i => i.Value))
                {
                    if (operation.parameters == null)
                    {
                        operation.parameters = new List<Parameter>();
                    }
                    else
                    {
                        foreach (var parameter in operation.parameters)
                        {
                            if (parameter.name == "subscriptionId")
                            {
                                parameter.name = null;
                                parameter.@in = null;
                                parameter.required = null;
                                parameter.type = null;
                                parameter.@ref = "#/parameters/subscriptionId";
                            }
                        }
                    }
                    operation.parameters.Add(new Parameter
                    {
                        @ref = "#/parameters/api-version"
                    });
                }
            }
        }

        // Dictionary<> class, when enumerated, returns entries in the order they were inserted (although that is not guaranteed)
        // To simplify diffs due to code changes, re-sort all dictionaries in alphabetical order
        private static void SortDocument(SwaggerDocument swaggerDoc)
        {
            MakeSorted(ref swaggerDoc.paths);
            MakeSorted(ref swaggerDoc.definitions);
            MakeSorted(ref swaggerDoc.parameters);
            MakeSorted(ref swaggerDoc.responses);
        }

        private static void MakeSorted<T>(ref IDictionary<string, T> field)
        {
            if (field == null)
            {
                return;
            }

            field = new SortedDictionary<string, T>(field);
        }
    }
}
