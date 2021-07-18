using System.Web.Http;
using WebActivatorEx;
using ElevenNote.WebAPI;
using Swashbuckle.Application;
using System.Linq;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;
using System.Web.Http.Filters;


[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace ElevenNote.WebAPI
{
    /// <summary>
    /// Document filter for adding Authorization header in Swashbuckle / Swa

    /// </summary>
    public class AddAuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)

        {
            var filterPipeline = apiDescription.ActionDescriptor.GetFilterPipeline();
            var isAuthorized = filterPipeline
                .Select(filterInfo => filterInfo.Instance)
                .Any(filter => filter is IAuthorizationFilter);
            var allowAnonymous = apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

            if (!isAuthorized || allowAnonymous) return;
            if (operation.parameters == null) operation.parameters = new List<Parameter>();

            operation.parameters.Add(new Parameter
            {
                name = "Authorization",
                @in = "header",
                description = "from /token endpoint",
                required = true,
                type = "string"
            });
        }
    }
    /// <summary>
    /// Document filter for adding OAuth Token endpoint documentation in Swa    
    /// Swagger normally won't find it - the /token endpoint - due to it beiprogrammatically generated.
    /// </summary>
    class AuthTokenEndpointOperation : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)

        {
            swaggerDoc.paths.Add("/token", new PathItem
            {
                post = new Operation
                {
                    tags = new List<string> { "Auth" },
                    consumes = new List<string>
                    {
                        "application/x-www-form-urlencoded"
                    },
                    parameters = new List<Parameter> { new Parameter
                    {
                        type = "string",
                        name = "grant_type",
                        required = true,
                        @in = "formData"
                    },
                        new Parameter
                        {
                            type = "string",
                            name = "username",
                            required = false,
                            @in = "formData"
                        },
                        new Parameter
                        {
                            type = "string",
                            name = "password",
                            required = false,
                            @in = "formData"
                        }
                    }
                }
            });
        }
    }
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;
            GlobalConfiguration.Configuration
            .EnableSwagger(c =>
            {
                // By default, the service root url is inferred from the request used to access the docs.

                // However, there may be situations (e.g. proxy and d - balanced environments) where this does not

                // resolve correctly. You can workaround this by providing your own code to determine the root URL.
                //
                //c.RootUrl(req => GetRootUrlFromAppConfig());
                // If schemes are not explicitly provided in a Swagg 2.0 document, then the scheme used to access
                // the docs is taken as the default. If your API sup ts multiple schemes and you want to be explicit
                // about them, you can use the "Schemes" option as shown  below.
                //
                //c.Schemes(new[] { "http", "https" });
                // Use "SingleApiVersion" to describe a single version API.Swagger 2.0 includes an "Info" object to
                // hold additional metadata for an API. Version and  le are required but you can also provide
                // additional fields by chaining methods off SingleApi Version.
                //c.SingleApiVersion("v1", "ElevenNote.WebAPI");
                // Enable adding the Authorization header to Authorization header to [Authorized endpoints. c.OperationFilter(() => new AddAuthorizationHeaderParameters
                //OperationFilter());
                // Show the programmatically generated /token endpoint the UI.
                //c.DocumentFilter<AuthTokenEndpointOperation>();
                // If your API has multiple versions, use "Multiple versions" instead of "SingleApiVersion".

                // In this case, you must provide a lambda that tell washbuckle which actions should be

                // included in the docs for a given API version. Like "SingleApiVersion", each call to "Version"

                // returns an "Info" builder so you can provide additional metadata per API version.

                //
                //c.MultipleApiVersions(
                // (apiDesc, targetApiVersion) => ResolveVersionSortByRouteConstraint(apiDesc, targetApiVersion),

                // (vc) =>
                // {
                // vc.Version("v2", "Swashbuckle Dummy API V2
                // vc.Version("v1", "Swashbuckle Dummy API V1
                // });
                // You can use "BasicAuth", "ApiKey" or "OAuth2" opt  s to describe security schemes for the API.

                // See https://github.com/swagger-api/swagger-spec/b  b / master / versions / 2.0.md for more details.

                // NOTE: These only define the schemes and need to be  coupled with a corresponding "security" property

                // at the document or operation level to indicate when  schemes are required for an operation. To do this,

                // you'll need to implement a custom IDocumentFilter  d / or IOperationFilter to set these properties

                // according to your specific authorization implemenention

                //
                //c.BasicAuth("basic")
                // .Description("Basic HTTP Authentication");
                //
                // NOTE: You must also configure 'EnableApiKeySupported below in the SwaggerUI section

                //c.ApiKey("apiKey")
                // .Description("API Key Authentication")
                // .Name("apiKey")
                // .In("header");
                //
                //c.OAuth2("oauth2")
                // .Description("OAuth2 Implicit Grant")
                // .Flow("implicit")
                // .AuthorizationUrl("http://petstore.swagger.work.com / api / oauth / dialog")

                // //.TokenUrl("https://tempuri.org/token")
                // .Scopes(scopes => // {
                // scopes.Add("read", "Read access to protect  resources");

                // scopes.Add("write", "Write access to protected resources");

                // });
                // Set this flag to omit descriptions for any action decorated with the Obsolete attribute

                //c.IgnoreObsoleteActions();
                // Each operation be assigned one or more tags which  e then used by consumers for various reasons.

                // For example, the swagger-ui groups operations acc ing to the first tag of each operation.

                // By default, this will be controller name but you  use the "GroupActionsBy" option to

                // override with any value.
                //
                //c.GroupActionsBy(apiDesc => apiDesc.HttpMethod.ToS ng()) ;

                // You can also specify a custom sort order for group (as defined by "GroupActionsBy") to dictate

                // the order in which operations are listed. For example, if the default grouping is in place

                // (controller name) and you specify a descending al betic sort order, then actions from a

                // ProductsController will be listed before those from  a CustomersController.This is typically

                // used to customize the order of groupings in the s ger - ui.

                //
                //c.OrderActionGroupsBy(new DescendingAlphabeticComp r());

                // If you annotate Controllers and API Types with
                // Xml comments (http://msdn.microsoft.com/en-us/lib  y / b2s063f7(v = vs.110).aspx), you can incorporate

                // those comments into the generated docs and UI. You  an enable this by providing the path to one or

                // more Xml comment files.
                //
                //c.IncludeXmlComments(GetXmlCommentsPath());
                // Swashbuckle makes a best attempt at generating Saggwer compliant JSON schemas for the various types

                // exposed in your API. However, there may be occasions when more control of the output is needed. ilter" options:
                // This is supported through the "MapType" and "Sche

                //
                // Use the "MapType" option to override the Schema g ration for a specific type.

                // It should be noted that the resulting Schema will  placed "inline" for any applicable Operations.

                // While Swagger 2.0 supports inline definitions for  ll" Schema types, the swagger-ui tool does not.

                // It expects "complex" Schemas to be defined separate y and referenced.For this reason, you should only

                // use the "MapType" option when the resulting Schema  s a primitive or array type.If you need to alter a

                // complex Schema, use a Schema filter.
                //
                //c.MapType<ProductType>(() => new Schema { type = "eger", format = "int32" });

                // If you want to post-modify "complex" Schemas once  ey've been generated, across the board or for a

                // specific type, you can wire up one or more Schema  lters.

                //
                //c.SchemaFilter<ApplySchemaVendorExtensions>();
                // In a Swagger 2.0 document, complex types are typing ly declared globally and referenced by unique

                // Schema Id. By default, Swashbuckle does NOT use the full type name in Schema Ids.In most cases, this

                // works well because it prevents the "implementation etail" of type namespaces from leaking into your

                // Swagger docs and UI. However, if you have multiple ypes in your API with the same class name, you'll

                // need to opt out of this behavior to avoid Schema conflicts.

                //
                //c.UseFullTypeNameInSchemaIds();
                // Alternatively, you can provide your own custom straegy for inferring SchemaId's for

                // describing "complex" types in your API.
                //
                //c.SchemaId(t => t.FullName.Contains('`') ? t.FullName.Substring(0, t.FullName.IndexOf('`')) : t.FullName);

                // Set this flag to omit schema property description or any type properties decorated with the

                // Obsolete attribute
                //c.IgnoreObsoleteProperties();  7 / 18/2021 EN 6.01 Swagger Setup Code: SD 92 6-14-21 Blue FT

                //https://elevenfifty.instructure.com/courses/734/pages/en-6-dot-01-swagger-setup-code?module_item_id=63564 7/10
                // In accordance with the built in JsonSerializer, Shbuckle will, by default, describe enums as integers.

                // You can change the serializer behavior by configure g the StringToEnumConverter globally or for a given

                // enum type. Swashbuckle will honor this change out the - box.However, if you use a different

                // approach to serialize enums as strings, you can a  force Swashbuckle to describe them as strings.

                //
                //c.DescribeAllEnumsAsStrings();
                // Similar to Schema filters, Swashbuckle also support Operation and Document filters:

                //
                // Post-modify Operation descriptions once they've be  generated by wiring up one or more

                // Operation filters.
                //
                //c.OperationFilter<AddDefaultResponse>();
                //
                // If you've defined an OAuth2 flow as described about you could use a custom filter

                // to inspect some attribute on each action and infe hich(if any) OAuth2 scopes are required

                // to execute the operation
                //
                //c.OperationFilter<AssignOAuth2SecurityRequirements();

                // Post-modify the entire Swagger document by wiring  one or more Document filters.

                // This gives full control to modify the final Swagger ocument.You should have a good understanding of

                // the Swagger 2.0 spec. - https://github.com/swagger pi / swagger-spec/blob/master/versions/2.0.md

                // before using this option.
                //
                //c.DocumentFilter<ApplyDocumentVendorExtensions>();
                // In contrast to WebApi, Swagger 2.0 does not include  the query string component when mapping a URL

                // to an action. As a result, Swashbuckle will raise  exception if it encounters multiple actions

                // with the same path (sans query string) and HTTP m od.You can workaround this by providing a

                // custom strategy to pick a winner or merge the des  ptions for the purposes of the Swagger docs 7 / 18 / 2021 EN 6.01 Swagger Setup Code: SD 92 6 - 14 - 21 Blue FT


                //https://elevenfifty.instructure.com/courses/734/pages/en-6-dot-01-swagger-setup-code?module_item_id=63564 8/10 c.ResolveConflictingActions(apiDescriptions => apiDe              
                // Wrap the default SwaggerGenerator with additional havior(e.g.caching) or provide an

                // alternative implementation for ISwaggerProvider w the CustomProvider option.
                //

                //c.CustomProvider((defaultProvider) => new CachingSgerProvider(defaultProvider));

            })
    .EnableSwaggerUi(c =>
            {
                // Use the "InjectStylesheet" option to enrich the U ith one or more additional CSS stylesheets.

                // The file must be included in your project as an "edded Resource", and then the resource's

                 // "Logical Name" is passed to the method as shown b

                 //
                   //c.InjectStylesheet(containingAssembly, "Swashbuckle ummy.SwaggerExtensions.testStyles1.css");

                 // Use the "InjectJavaScript" option to invoke one o ore custom JavaScripts after the swagger-ui

                 // has loaded. The file must be included in your pro t as an "Embedded Resource", and then the resource's

                // "Logical Name" is passed to the method as shown a

                //
                //c.InjectJavaScript(thisAssembly, "Swashbuckle.Dumm waggerExtensions.testScript1.js");

                // The swagger-ui renders boolean data types as a dr own.By default, it provides "true" and "false"

                // strings as the possible choices. You can use this tion to change these to something else,
                // for example 0 and 1.
                //
                //c.BooleanValues(new[] { "0", "1" });
                // By default, swagger-ui will validate specs agains wagger.io's online validator and display the result

                // in a badge at the bottom of the page. Use these o  ons to set a different validator URL or to disable the

                // feature entirely.
                //c.SetValidatorUrl("http://localhost/validator");
                //c.DisableValidator();  7 / 18 / 2021 EN 6.01 Swagger Setup Code: SD 92 6 - 14 - 21 Blue FT

                //https://elevenfifty.instructure.com/courses/734/pages/en-6-dot-01-swagger-setup-code?module_item_id=63564 9/10
                // Use this option to control how the Operation list is displayed.

                // It can be set to "None" (default), "List" (shows rations for each resource),

                // or "Full" (fully expanded: shows operations and the r details).

                //
                //c.DocExpansion(DocExpansion.List);
                // Specify which HTTP operations will have the 'Try out!' option. An empty paramter list disables

                // it for all operations.
                 //
                //c.SupportedSubmitMethods("GET", "HEAD");
                // Use the CustomAsset option to provide your own version  of assets used in the swagger-ui.

                // It's typically used to instruct Swashbuckle to re n your version instead of the default

                // when a request is made for "index.html". As with

                // in your project as an "Embedded Resource", and th custom content, the file must be included  the resource's "Logical Name" is passed to

                // the method as shown below.
                //
                //c.CustomAsset("index", containingAssembly, "YourWe  iProject.SwaggerExtensions.index.html");

                // If your API has multiple versions and you've appl the MultipleApiVersions setting

                // as described above, you can also enable a select in the swagger-ui, that displays

                // a discovery URL for each version. This provides a nvenient way for users to browse documentation

                // for different API versions.
                //
                //c.EnableDiscoveryUrlSelector();
                // If your API supports the OAuth2 Implicit flow, an ou've described it correctly, according to

                // the Swagger 2.0 specification, you can enable UI  port as shown below.

                //
                //c.EnableOAuth2Support(
                // clientId: "test-client-id",
                // clientSecret: null,
                // realm: "test-realm",
                // appName: "Swagger UI"
                // //additionalQueryStringParams: new Dictionary< 7 / 18 / 2021 EN 6.01 Swagger Setup Code: SD 92 6 - 14 - 21 Blue FT

                //https://elevenfifty.instructure.com/courses/734/pages/en-6-dot-01-swagger-setup-code?module_item_id=63564 10/10 ing, string > () { { "foo", "bar" } }

                //);
                // If your API supports ApiKey, you can override the  fault values.

                // "apiKeyIn" can either be "query" or "header"
                //
                //c.EnableApiKeySupport("apiKey", "
            });

        }
    }
}




