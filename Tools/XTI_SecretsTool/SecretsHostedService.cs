﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_Credentials;
using XTI_Secrets;
using XTI_SecretsToolApi;
using XTI_Tool;

namespace XTI_SecretsTool;

public sealed class SecretsHostedService : IHostedService
{
    private readonly IServiceProvider services;

    public SecretsHostedService(IServiceProvider services)
    {
        this.services = services;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        try
        {
            var options = scope.ServiceProvider.GetRequiredService<SecretsToolOptions>();
            if (string.IsNullOrWhiteSpace(options.CredentialKey))
            {
                throw new ArgumentException("Credential Key is Required");
            }
            if (string.IsNullOrWhiteSpace(options.Command) || options.Command.Equals("Store", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(options.UserName))
                {
                    throw new ArgumentException("User Name is Required");
                }
                if (string.IsNullOrWhiteSpace(options.Password))
                {
                    throw new ArgumentException("Password is Required");
                }
                var credentialFactory = getSecretCredentialsFactory(scope, options);
                var secretCredentials = credentialFactory.Create(options.CredentialKey);
                await secretCredentials.Update(new CredentialValue(options.UserName, options.Password));
                Console.WriteLine($"Secrets stored for user {options.UserName}");
            }
            else if (options.Command.Equals("Get", StringComparison.OrdinalIgnoreCase))
            {
                var credentialFactory = getSecretCredentialsFactory(scope, options);
                var secretCredentials = credentialFactory.Create(options.CredentialKey);
                var credentials = await secretCredentials.Value();
                new XtiProcessData().Output(new SecretsToolOutput
                {
                    UserName = credentials.UserName,
                    Password = credentials.Password
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Environment.ExitCode = 999;
        }
        var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
        lifetime.StopApplication();
    }

    private static ISecretCredentialsFactory getSecretCredentialsFactory(IServiceScope scope, SecretsToolOptions options)
    {
        ISecretCredentialsFactory credentialFactory;
        if (options.IsShared)
        {
            credentialFactory = scope.ServiceProvider.GetRequiredService<ISharedSecretCredentialsFactory>();
        }
        else
        {
            credentialFactory = scope.ServiceProvider.GetRequiredService<SecretCredentialsFactory>();
        }
        return credentialFactory;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}