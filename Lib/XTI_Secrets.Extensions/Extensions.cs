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
        public static void AddXtiDataProtection(this IServiceCollection services)
        {
            const string appName = "XTI_App";
            var keyDirPath = new AppDataFolder()
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

        public static void AddFileSecretCredentials(this IServiceCollection services)
        {
            services.AddSingleton<SecretCredentialsFactory>(sp =>
            {
                var hostEnv = sp.GetService<IHostEnvironment>();
                var dataProtector = sp.GetDataProtector(new[] { "XTI_Secrets" });
                return new FileSecretCredentialsFactory(hostEnv, dataProtector);
            });
        }

        public static void AddSharedFileSecretCredentials(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var dataProtector = sp.GetDataProtector(new[] { "XTI_Secrets" });
                return new SharedFileSecretCredentialsFactory(dataProtector);
            });
        }
    }
}
