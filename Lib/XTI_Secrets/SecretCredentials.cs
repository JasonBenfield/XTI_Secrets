using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;
using XTI_Credentials;

namespace XTI_Secrets;

public abstract class SecretCredentials : ICredentials
{
    private readonly string key;
    private readonly IDataProtector dataProtector;

    protected SecretCredentials(string key, IDataProtector dataProtector)
    {
        this.key = key;
        this.dataProtector = dataProtector;
    }

    public void Delete()
    {
        if (Exist())
        {
            _Delete(key);
        }
    }

    protected abstract void _Delete(string key);

    public Task Update(string userName, string password) =>
        Update(new CredentialValue(userName, password));

    public Task Update(CredentialValue value)
    {
        var serialized = JsonSerializer.Serialize(new CredentialValueRecord(value));
        var encrypted = new EncryptedValue(dataProtector, serialized).Value();
        return Persist(key, encrypted);
    }

    protected abstract Task Persist(string key, string encryptedText);

    public bool Exist() => _Exist(key);

    protected abstract bool _Exist(string key);

    public async Task<CredentialValue> Value()
    {
        var encrypted = await Load(key);
        var serialized = new DecryptedValue(dataProtector, encrypted).Value();
        CredentialValueRecord deserialized;
        if (string.IsNullOrWhiteSpace(serialized))
        {
            deserialized = new CredentialValueRecord();
        }
        else
        {
            deserialized = JsonSerializer.Deserialize<CredentialValueRecord>(serialized)
                ?? new CredentialValueRecord();
        }
        return new CredentialValue(deserialized.UserName, deserialized.Password);
    }

    protected abstract Task<string> Load(string key);

    private class CredentialValueRecord
    {
        private string userName = "";
        private string password = "";

        public CredentialValueRecord()
            : this(new CredentialValue("", ""))
        {
        }

        public CredentialValueRecord(CredentialValue credentialValue)
        {
            UserName = credentialValue.UserName;
            Password = credentialValue.Password;
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
    }
}