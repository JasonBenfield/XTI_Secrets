using Microsoft.AspNetCore.DataProtection;
using XTI_Core;

namespace XTI_Secrets.Files
{
    public sealed class FileSecretCredentialsFactory : SecretCredentialsFactory
    {
        private readonly XtiFolder xtiFolder;

        public FileSecretCredentialsFactory(XtiFolder xtiFolder, IDataProtector dataProtector)
            : base(dataProtector)
        {
            this.xtiFolder = xtiFolder;
        }

        protected override SecretCredentials _Create(string key, IDataProtector dataProtector) =>
            new FileSecretCredentials(xtiFolder, key, dataProtector);
    }
}
