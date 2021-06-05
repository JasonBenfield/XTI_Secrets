using Microsoft.AspNetCore.DataProtection;
using System.Threading.Tasks;

namespace XTI_Secrets.Fakes
{
    public sealed class FakeSecretCredentials : SecretCredentials
    {
        public FakeSecretCredentials(string key, IDataProtector dataProtector) : base(key, dataProtector)
        {
        }

        public string StoredText { get; private set; }

        public bool IsDeleted { get; private set; }

        protected override void _Delete(string key)
        {
            IsDeleted = true;
            StoredText = null;
        }

        protected override bool _Exist(string key) => !string.IsNullOrWhiteSpace(StoredText);

        protected override Task<string> Load(string key) => Task.FromResult(StoredText);

        protected override Task Persist(string key, string encryptedText)
        {
            StoredText = encryptedText;
            return Task.CompletedTask;
        }
    }
}
