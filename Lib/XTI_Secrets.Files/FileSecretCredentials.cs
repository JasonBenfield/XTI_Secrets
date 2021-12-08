using Microsoft.AspNetCore.DataProtection;
using XTI_Core;

namespace XTI_Secrets.Files;

public sealed class FileSecretCredentials : SecretCredentials
{
    private readonly SharedFileSecretCredentials sharedCredentials;
    private readonly AppDataFolder appDataFolder;

    internal FileSecretCredentials(XtiFolder xtiFolder, string key, IDataProtector dataProtector)
        : base(key, dataProtector)
    {
        sharedCredentials = new SharedFileSecretCredentials(xtiFolder, key, dataProtector);
        appDataFolder = xtiFolder
            .AppDataFolder()
            .WithSubFolder("Secrets");
    }

    protected override void _Delete(string key) => File.Delete(getFilePath(key));

    protected override bool _Exist(string key)
    {
        var filePath = getFilePath(key);
        var exists = File.Exists(filePath);
        if (!exists)
        {
            exists = sharedCredentials.Exist();
        }
        return exists;
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
            text = await sharedCredentials.LoadShared(key);
        }
        return text;
    }

    protected override async Task Persist(string key, string encryptedText)
    {
        appDataFolder.TryCreate();
        using (var writer = new StreamWriter(getFilePath(key), false))
        {
            await writer.WriteAsync(encryptedText);
        }
    }

    private string getFilePath(string key) => appDataFolder.FilePath($"{key}.secret");
}