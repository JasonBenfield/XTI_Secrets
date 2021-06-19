namespace XTI_Secrets
{
    public interface ISecretCredentialsFactory
    {
        SecretCredentials Create(string key);
    }
}
