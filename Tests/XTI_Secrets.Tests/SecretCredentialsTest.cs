using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Text.Json;
using XTI_Core.Extensions;
using XTI_Credentials;
using XTI_Secrets.Fakes;

namespace XTI_Secrets.Tests;

internal sealed class SecretCredentialsTest
{
    [Test]
    public async Task ShouldStoreAndRetrieveCredentials()
    {
        var sp = setup();
        var factory = getSecretCredentailsFactory(sp);
        var secretCredentials = factory.Create("Test");
        var storedCredentials = new CredentialValue("Someone", "Password");
        await secretCredentials.Update(storedCredentials);
        var retrievedCredentials = await secretCredentials.Value();
        Assert.That(retrievedCredentials, Is.EqualTo(storedCredentials), "Should store and retrieve credentials");
    }

    [Test]
    public async Task ShouldEncryptCredentials()
    {
        var sp = setup();
        var factory = getSecretCredentailsFactory(sp);
        var secretCredentials = factory.Create("Test");
        var storedCredentials = new CredentialValue("Someone", "Password");
        await secretCredentials.Update(storedCredentials);
        Assert.That
        (
            secretCredentials.StoredText,
            Is.Not.EqualTo(JsonSerializer.Serialize(storedCredentials)),
            "Should encrypt credentials"
        );
    }

    private IServiceProvider setup()
    {
        var builder = new XtiHostBuilder();
        builder.Services.AddFakeSecretCredentials();
        return builder.Build().Scope();
    }

    private FakeSecretCredentialsFactory getSecretCredentailsFactory(IServiceProvider sp)
        => (FakeSecretCredentialsFactory)sp.GetRequiredService<SecretCredentialsFactory>();
}