using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Configuration;
using GraphQLFunction.Services;
using GraphQLFunction;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQLFunction
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(s =>
                {
                    s.AddTransient<IUserRepository, UserRepository>();
                    s.AddSingleton<GraphQLAzureFunctionsExecutorProxyV12>();
                    s.AddGraphQLServer()
                    .AddQueryType<Query>()
                    .AddFiltering()
                    .AddSorting();

                }).Build();

            host.Run();
        }
    }
}