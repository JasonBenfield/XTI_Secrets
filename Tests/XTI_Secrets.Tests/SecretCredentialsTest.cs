using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using XTI_Credentials;
using XTI_Secrets.Fakes;

namespace XTI_Secrets.Tests
{
    public sealed class SecretCredentialsTest
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
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices
                (
                    services =>
                    {
                        services.AddFakeSecretCredentials();
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            return scope.ServiceProvider;
        }

        private FakeSecretCredentialsFactory getSecretCredentailsFactory(IServiceProvider sp)
            => (FakeSecretCredentialsFactory)sp.GetService<SecretCredentialsFactory>();
    }
}
