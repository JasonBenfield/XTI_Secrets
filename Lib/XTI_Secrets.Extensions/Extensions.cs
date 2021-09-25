using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using XTI_Core;
using XTI_Secrets.Files;

namespace XTI_Secrets.Extensions
{
    public static class Extensions
    {
        public static void AddFileSecretCredentials(this IServiceCollection services, IHostEnvironment hostEnv)
        {
            services.AddXtiDataProtection(hostEnv);
            services.AddSingleton<ISecretCredentialsFactory>(sp =>
            {
                var xtiFolder = sp.GetService<XtiFolder>();
                var dataProtector = sp.GetDataProtector(new[] { "XTI_Secrets" });
                return new FileSecretCredentialsFactory(xtiFolder, dataProtector);
            });
            services.AddSingleton(sp => (SecretCredentialsFactory)sp.GetService<ISecretCredentialsFactory>());
            services.AddSingleton<SharedSecretCredentialsFactory>(sp =>
            {
                var xtiFolder = sp.GetService<XtiFolder>();
                var dataProtector = sp.GetDataProtector(new[] { "XTI_Secrets" });
                return new SharedFileSecretCredentialsFactory(xtiFolder, dataProtector);
            });
        }

        public static void AddXtiDataProtection(this IServiceCollection services, IHostEnvironment hostEnv)
        {
            const string appName = "XTI_App";
            var keyDirPath = new XtiFolder(hostEnv)
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
}
