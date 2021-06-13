using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQLFunction
{
    public class GraphQLAzureFunctionsExecutorProxyV12
    {
        private readonly IHttpRequestParser _requestParser;
        private readonly IHttpResultSerializer _resultSerializer;
        private readonly IRequestExecutorResolver _requestExecutorResolver;
        public GraphQLAzureFunctionsExecutorProxyV12(
            IHttpRequestParser requestParser,
            IHttpResultSerializer resultSerializer,
            IRequestExecutorResolver requestExecutorResolver)
        {
            _requestParser = requestParser;
            _resultSerializer = resultSerializer;
            _requestExecutorResolver = requestExecutorResolver;
        }

        public async Task<HttpResponseData> ExecuteQueryAsync(HttpRequestData req)
        {
            IExecutionResult result = default;
            try
            {
                var requests = await _requestParser.ReadJsonRequestAsync(req.Body, CancellationToken.None).ConfigureAwait(false);
                var executor = await _requestExecutorResolver.GetRequestExecutorAsync().ConfigureAwait(false);
                var batchRequest = requests.Select(r => QueryRequestBuilder.From(r).Create());
                result = await executor.ExecuteBatchAsync(batchRequest).ConfigureAwait(false);
            }
            catch (GraphQLRequestException ex)
            {
                result = QueryResultBuilder.CreateError(ex.Errors);
            }
            catch (Exception ex)
            {
                result = QueryResultBuilder.CreateError(new Error(ex.Message));
            }

            return await PrepareResponse(req, result).ConfigureAwait(false);
        }

        private async Task<HttpResponseData> PrepareResponse(HttpRequestData requestData, IExecutionResult queryResult)
        {
            var response = requestData.CreateResponse();
            response.Headers.Add("Content-Type", _resultSerializer.GetContentType(queryResult));
            response.StatusCode = _resultSerializer.GetStatusCode(queryResult);
            var json = await SerializeResult(queryResult).ConfigureAwait(false);
            await response.WriteStringAsync(json).ConfigureAwait(false);
            return response;
        }

        private static async Task<string> SerializeResult(IExecutionResult queryResult)
        {
            if (queryResult is IBatchQueryResult batchQueryResult)
            {
                var sb = new StringBuilder();
                await foreach (IQueryResult payload in batchQueryResult.ReadResultsAsync())
                {
                    var json = await payload.ToJsonAsync().ConfigureAwait(false);
                    sb.AppendLine(json);
                }

                return sb.ToString();
            }

            return await queryResult.ToJsonAsync().ConfigureAwait(false);
        }
    }


}
