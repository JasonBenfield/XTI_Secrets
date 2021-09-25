using Microsoft.AspNetCore.DataProtection;
using XTI_Core;

namespace XTI_Secrets.Files
{
    public sealed class SharedFileSecretCredentialsFactory : SecretCredentialsFactory, SharedSecretCredentialsFactory
    {
        private readonly XtiFolder xtiFolder;

        public SharedFileSecretCredentialsFactory(XtiFolder xtiFolder, IDataProtector dataProtector)
            : base(dataProtector)
        {
            this.xtiFolder = xtiFolder;
        }

        protected override SecretCredentials _Create(string key, IDataProtector dataProtector) =>
            new SharedFileSecretCredentials(xtiFolder, key, dataProtector);
    }
}
