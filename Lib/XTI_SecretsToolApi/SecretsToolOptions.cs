namespace XTI_SecretsToolApi
{
    public sealed class SecretsToolOptions
    {
        public string Command { get; set; }
        public string CredentialKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
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
            Command = "Store";
            CredentialKey = credentialKey;
        }
    }
}
