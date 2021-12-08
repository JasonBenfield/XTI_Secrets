namespace XTI_Credentials;

public sealed class SimpleCredentials : ICredentials
{
    private readonly CredentialValue value;

    public SimpleCredentials(CredentialValue value)
    {
        this.value = value;
    }

    public Task<CredentialValue> Value() => Task.FromResult(value);
}