using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using XTI_Core;
using XTI_Core.Extensions;
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
        var hostBuilder = new XtiHostBuilder();
        hostBuilder.Services.AddFileSecretCredentials();
        var host = hostBuilder.Build();
        var xtiEnv = host.GetRequiredService<XtiEnvironmentAccessor>();
        xtiEnv.UseTest();
        return host.Scope();
    }

    private ISecretCredentialsFactory getSecretCredentialsFactory(IServiceProvider sp)
        => sp.GetRequiredService<ISecretCredentialsFactory>();
}