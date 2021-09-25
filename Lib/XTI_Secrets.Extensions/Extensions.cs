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
            services
                .AddDataProtection
                (
                    options => options.ApplicationDiscriminator = appName
                )
                .SetApplicationName(appName);
        }

        public static void AddFileSecretCredentials(this IServiceCollection services)
        {
            services.AddXtiDataProtection();
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
    }
}
