using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Text.Json;
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

    [Test]
    public async Task ShouldInstallation()
    {
        var sp = setup("Development");
        var factory = getSecretCredentialsFactory(sp);
        var secretCredentials = factory.Create("Installation");
        var retrievedCredentials = await secretCredentials.Value();
        Console.WriteLine(JsonSerializer.Serialize(retrievedCredentials, new JsonSerializerOptions { WriteIndented = true}));
    }

    private IServiceProvider setup(string envName = "Test")
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", envName);
        var xtiEnv = XtiEnvironment.Parse(envName);
        var hostBuilder = new XtiHostBuilder(xtiEnv);
        hostBuilder.Services.AddFileSecretCredentials(xtiEnv);
        var host = hostBuilder.Build();
        return host.Scope();
    }

    private ISecretCredentialsFactory getSecretCredentialsFactory(IServiceProvider sp)
        => sp.GetRequiredService<ISecretCredentialsFactory>();
}