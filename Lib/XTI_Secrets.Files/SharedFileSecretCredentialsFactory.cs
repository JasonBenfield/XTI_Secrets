using Microsoft.AspNetCore.DataProtection;

namespace XTI_Secrets.Files
{
    public sealed class SharedFileSecretCredentialsFactory : SecretCredentialsFactory, SharedSecretCredentialsFactory
    {
        public SharedFileSecretCredentialsFactory(IDataProtector dataProtector)
            : base(dataProtector)
        {
        }

        protected override SecretCredentials _Create(string key, IDataProtector dataProtector) =>
            new FileSecretCredentials("Shared", key, dataProtector);
    }
}
