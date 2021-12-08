namespace XTI_SecretsToolApi;

public sealed class SecretsToolOutput
{
    private string userName = "";
    private string password = "";

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
}