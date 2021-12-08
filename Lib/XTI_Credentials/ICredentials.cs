namespace XTI_Credentials;

public interface ICredentials
{
    Task<CredentialValue> Value();
}