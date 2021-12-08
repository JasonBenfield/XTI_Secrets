namespace XTI_SecretsToolApi;

public sealed class SecretsToolOptions
{
    private string command = "";
    private string credentialKey = "";
    private string userName = "";
    private string password = "";

    public string Command
    {
        get => command;
        set => command = value ?? "";
    }

    public string CredentialKey
    {
        get => credentialKey;
        set => credentialKey = value ?? "";
    }

    public string UserName
    {
        get => userName;
        set => userName = value ?? "";
    }

    public string Password
    {
        get => password;
        set => password = value ?? "";
    }

    public bool IsShared { get; set; }

    public void CommandStore(string credentialKey, string userName, string password)
    {
        Command = "Store";
        CredentialKey = credentialKey;
        UserName = userName;
        Password = password;
    }

    public void CommandGet(string credentialKey)
    {
        Command = "Get";
        CredentialKey = credentialKey;
    }
}