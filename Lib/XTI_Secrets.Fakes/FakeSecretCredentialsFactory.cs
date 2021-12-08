using Microsoft.AspNetCore.DataProtection;

namespace XTI_Secrets.Fakes;

public sealed class FakeSecretCredentialsFactory : SecretCredentialsFactory
{
    private readonly Dictionary<string, FakeSecretCredentials> credentialLookup = new Dictionary<string, FakeSecretCredentials>();

    public FakeSecretCredentialsFactory(IDataProtector dataProtector)
        : base(dataProtector)
    {
    }

    public new FakeSecretCredentials Create(string key) => 
        (FakeSecretCredentials)base.Create(key);

    protected override SecretCredentials _Create(string key, IDataProtector dataProtector)
    {
        if (!credentialLookup.TryGetValue(key, out var secretCredentials))
        {
            secretCredentials = new FakeSecretCredentials(key, dataProtector);
            credentialLookup.Add(key, secretCredentials);
        }
        return secretCredentials;
    }
}