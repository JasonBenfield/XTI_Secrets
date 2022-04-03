using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_Credentials;
using XTI_Secrets.Extensions;

namespace XTI_Secrets.IntegrationTests;

internal sealed class SharedSecretCredentialsIntegrationTest
{
    [Test]
    public async Task ShouldRetrieveGitHubCredentials()
    {
        var sp = setup();
        var factory = getSecretCredentialsFactory(sp);
        var secretCredentials = factory.Create("GitHub");
        var cred = await secretCredentials.Value();
    }

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
    public async Task ShouldStoreAndRetrieveCredentialsFromAllEnvironments()
    {
        var storedCredentials = new CredentialValue("Someone", "Password");
        await storeCredentials("Test", storedCredentials);
        var retrievedCredentials = await retrieveCredentials("Development");
        Assert.That(retrievedCredentials, Is.EqualTo(storedCredentials), "Should retrieve shared credentials");
        retrievedCredentials = await retrieveCredentials("Staging");
        Assert.That(retrievedCredentials, Is.EqualTo(storedCredentials), "Should retrieve shared credentials");
        retrievedCredentials = await retrieveCredentials("Production");
        Assert.That(retrievedCredentials, Is.EqualTo(storedCredentials), "Should retrieve shared credentials");
    }

    private async Task storeCredentials(string envName, CredentialValue storedCredentials)
    {
        var sp = setup(envName);
        var factory = getSecretCredentialsFactory(sp);
        var secretCredentials = factory.Create("Test");
        secretCredentials.Delete();
        await secretCredentials.Update(storedCredentials);
    }

    private Task<CredentialValue> retrieveCredentials(string envName)
    {
        var sp = setup(envName);
        var factory = getSecretCredentialsFactory(sp);
        var secretCredentials = factory.Create("Test");
        return secretCredentials.Value();
    }

    private IServiceProvider setup(string envName = "Test")
    {
        var hostBuilder = new XtiHostBuilder();
        hostBuilder.Services.AddFileSecretCredentials();
        var host = hostBuilder.Build();
        var env = host.GetRequiredService<XtiEnvironmentAccessor>();
        env.UseEnvironment(envName);
        return host.Scope();
    }

    private ISharedSecretCredentialsFactory getSecretCredentialsFactory(IServiceProvider sp)
        => sp.GetRequiredService<ISharedSecretCredentialsFactory>();
}