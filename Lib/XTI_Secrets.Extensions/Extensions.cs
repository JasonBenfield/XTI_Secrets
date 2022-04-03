﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using XTI_Core;
using XTI_Secrets.Files;

namespace XTI_Secrets.Extensions;

public static class Extensions
{
    public static void AddFileSecretCredentials(this IServiceCollection services) =>
        services.AddFileSecretCredentials(XtiEnvironment.Shared);

    public static void AddFileSecretCredentials(this IServiceCollection services, XtiEnvironment environment)
    {
        services.AddXtiDataProtection(environment);
        services.AddScoped<ISecretCredentialsFactory>(sp =>
        {
            var xtiFolder = sp.GetRequiredService<XtiFolder>();
            var dataProtector = sp.GetDataProtector(new[] { "XTI_Secrets" });
            return new FileSecretCredentialsFactory(xtiFolder, dataProtector);
        });
        services.AddScoped(sp => (SecretCredentialsFactory)sp.GetRequiredService<ISecretCredentialsFactory>());
        services.AddScoped<ISharedSecretCredentialsFactory>(sp =>
        {
            var xtiFolder = sp.GetRequiredService<XtiFolder>();
            var dataProtector = sp.GetDataProtector(new[] { "XTI_Secrets" });
            return new SharedFileSecretCredentialsFactory(xtiFolder, dataProtector);
        });
    }

    public static void AddXtiDataProtection(this IServiceCollection services) =>
        services.AddXtiDataProtection(XtiEnvironment.Shared);

    public static void AddXtiDataProtection(this IServiceCollection services, XtiEnvironment environment)
    {
        const string appName = "XTI_App";
        var keyDirPath = new XtiFolder(environment)
            .SharedAppDataFolder()
            .WithSubFolder("Keys")
            .Path();
        services
            .AddDataProtection
            (
                options => options.ApplicationDiscriminator = appName
            )
            .PersistKeysToFileSystem(new DirectoryInfo(keyDirPath))
            .SetApplicationName(appName);
    }
}