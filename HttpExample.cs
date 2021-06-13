using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using GraphQLFunction;

namespace GraphQL.Function
{
    public partial class HttpExample
    {
        private readonly GraphQLAzureFunctionsExecutorProxyV12 _graphQlExecutorProxy;
        public HttpExample(GraphQLAzureFunctionsExecutorProxyV12 graphQlExecutorProxy)
        {
            _graphQlExecutorProxy = graphQlExecutorProxy;
        }        

        [Function(nameof(GraphQL))]
        public async Task<HttpResponseData> GraphQL(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "graphql")] HttpRequestData req)
            => await _graphQlExecutorProxy.ExecuteQueryAsync(req).ConfigureAwait(false);
    }
}
