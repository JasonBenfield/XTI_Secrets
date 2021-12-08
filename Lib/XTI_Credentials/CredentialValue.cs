namespace XTI_Credentials;

public sealed class CredentialValue : IEquatable<CredentialValue>
{
    private readonly int hashCode;

    public CredentialValue(string userName, string password)
    {
        UserName = userName;
        Password = password;
        hashCode = $"{UserName}|{Password}".GetHashCode();
    }

    public string UserName { get; }
    public string Password { get; }

    public override bool Equals(object? obj) => Equals(obj as CredentialValue);

    public bool Equals(CredentialValue? other) =>
        UserName == other?.UserName && Password == other?.Password;

    public override int GetHashCode() => hashCode;

    public override string ToString() => $"{nameof(CredentialValue)} {UserName}";

}