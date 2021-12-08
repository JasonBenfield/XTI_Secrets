using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_Configuration.Extensions;
using XTI_Core;
using XTI_Secrets.Extensions;
using XTI_SecretsTool;
using XTI_SecretsToolApi;

await Host.CreateDefaultBuilder(args)
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
            services.AddFileSecretCredentials(hostContext.HostingEnvironment);
            services.AddHostedService<SecretsHostedService>();
        }
    )
    .Build()
    .RunAsync();