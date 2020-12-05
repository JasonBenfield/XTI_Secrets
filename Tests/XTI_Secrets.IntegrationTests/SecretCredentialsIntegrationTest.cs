using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using XTI_Configuration.Extensions;
using XTI_Credentials;
using XTI_Secrets.Extensions;

namespace XTI_Secrets.IntegrationTests
{
    public class SecretCredentialsIntegrationTest
    {
        [Test]
        public async Task ShouldStoreAndRetrieveCredentials()
        {
            var input = setup();
            var secretCredentials = input.Factory.Create("Test");
            var storedCredentials = new CredentialValue("Someone", "Password");
            await secretCredentials.Update(storedCredentials);
            var retrievedCredentials = await secretCredentials.Value();
            Assert.That(retrievedCredentials, Is.EqualTo(storedCredentials), "Should store and retrieve credentials");
        }

        private TestInput setup()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.UseXtiConfiguration(hostingContext.HostingEnvironment, new string[] { });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddXtiDataProtection();
                    services.AddFileSecretCredentials();
                })
                .Build();
            var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            var input = new TestInput(sp);
            return input;
        }

        private sealed class TestInput
        {
            public TestInput(IServiceProvider sp)
            {
                Factory = sp.GetService<SecretCredentialsFactory>();
            }

            public SecretCredentialsFactory Factory { get; }
        }
    }
}
