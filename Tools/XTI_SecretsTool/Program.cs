using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using XTI_Configuration.Extensions;
using XTI_Core;
using XTI_Secrets.Extensions;
using XTI_SecretsToolApi;

namespace XTI_SecretsTool
{
    class Program
    {
        static Task Main(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration
                (
                    (hostContext, configuration) =>
                    {
                        configuration.UseXtiConfiguration(hostContext.HostingEnvironment, args);
                    }
                )
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.Configure<SecretsToolOptions>(hostContext.Configuration);
                        services.AddSingleton<XtiFolder>();
                        services.AddFileSecretCredentials();
                        services.AddHostedService<SecretsHostedService>();
                    }
                )
                .Build()
                .RunAsync();
    }
}
