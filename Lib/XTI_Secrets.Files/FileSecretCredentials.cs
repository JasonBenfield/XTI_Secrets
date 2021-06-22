using Microsoft.AspNetCore.DataProtection;
using System;
using System.IO;
using System.Threading.Tasks;
using XTI_Core;

namespace XTI_Secrets.Files
{
    public sealed class FileSecretCredentials : SecretCredentials
    {
        private readonly AppDataFolder sharedAppDataFolder;
        private readonly AppDataFolder appDataFolder;

        internal FileSecretCredentials(string envName, string key, IDataProtector dataProtector)
            : base(key, dataProtector)
        {
            sharedAppDataFolder = new AppDataFolder()
                .WithSubFolder("Shared")
                .WithSubFolder("Secrets");
            appDataFolder = new AppDataFolder()
                .WithSubFolder(envName)
                .WithSubFolder("Secrets");
        }

        protected override void _Delete(string key) => File.Delete(getFilePath(key));

        protected override bool _Exist(string key)
        {
            var filePath = getFilePath(key);
            return File.Exists(filePath);
        }

        protected override async Task<string> Load(string key)
        {
            string text;
            var filePath = getFilePath(key);
            if (File.Exists(filePath))
            {
                using var reader = new StreamReader(filePath);
                text = await reader.ReadToEndAsync();
            }
            else
            {
                throw new ArgumentException("Secrets file was not found");
            }
            return text;
        }

        protected override async Task Persist(string key, string encryptedText)
        {
            appDataFolder.TryCreate();
            using (var writer = new StreamWriter(getExplicitFilePath(key), false))
            {
                await writer.WriteAsync(encryptedText);
            }
        }

        private string getFilePath(string key)
        {
            var filePath = getExplicitFilePath(key);
            if (!File.Exists(filePath))
            {
                filePath = getSharedFilePath(key);
            }
            return filePath;
        }

        private string getExplicitFilePath(string key) => appDataFolder.FilePath($"{key}.secret");

        private string getSharedFilePath(string key) => sharedAppDataFolder.FilePath($"{key}.secret");
    }
}
