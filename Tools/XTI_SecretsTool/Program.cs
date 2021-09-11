using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using XTI_Configuration.Extensions;
using XTI_SecretsToolApi;
using XTI_Secrets.Extensions;

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
                        services.AddXtiDataProtection();
                        services.AddFileSecretCredentials();
                        services.AddSharedFileSecretCredentials();
                        services.AddHostedService<SecretsHostedService>();
                    }
                )
                .Build()
                .RunAsync();
    }
}
