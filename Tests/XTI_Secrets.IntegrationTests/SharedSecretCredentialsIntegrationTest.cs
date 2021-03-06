﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using XTI_Configuration.Extensions;
using XTI_Credentials;
using XTI_Secrets.Extensions;
using XTI_Secrets.Files;

namespace XTI_Secrets.IntegrationTests
{
    public class SharedSecretCredentialsIntegrationTest
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
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", envName);
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.UseXtiConfiguration(hostingContext.HostingEnvironment, new string[] { });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddXtiDataProtection();
                    services.AddSharedFileSecretCredentials();
                })
                .Build();
            var scope = host.Services.CreateScope();
            return scope.ServiceProvider;
        }

        private SecretCredentialsFactory getSecretCredentialsFactory(IServiceProvider sp)
            => sp.GetService<SharedFileSecretCredentialsFactory>();
    }
}
