using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using XTI_Configuration.Extensions;
using XTI_Core;
using XTI_Credentials;
using XTI_Secrets.Extensions;

namespace XTI_Secrets.IntegrationTests;

internal sealed class SecretCredentialsIntegrationTest
{
    [Test]
    public async Task ShouldStoreAndRetrieveCredentials()
    {
        var sp = setup();
        var factory = getSecretCredentialsFactory(sp);
        var secretCredentials = factory.Create("Test");
        var storedCredentials = new CredentialValue("Someone", "Password");
        await secretCredentials.Update(storedCredentials);
        var retrievedCredentials = await secretCredentials.Value();
        Assert.That(retrievedCredentials, Is.EqualTo(storedCredentials), "Should store and retrieve credentials");
    }

    private IServiceProvider setup()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.UseXtiConfiguration(hostingContext.HostingEnvironment, new string[] { });
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<XtiFolder>();
                services.AddFileSecretCredentials(hostContext.HostingEnvironment);
            })
            .Build();
        var scope = host.Services.CreateScope();
        return scope.ServiceProvider;
    }

    private SecretCredentialsFactory getSecretCredentialsFactory(IServiceProvider sp)
        => sp.GetRequiredService<SecretCredentialsFactory>();
}