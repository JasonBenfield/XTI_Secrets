using Microsoft.AspNetCore.DataProtection;
using XTI_Core;

namespace XTI_Secrets.Files;

public sealed class SharedFileSecretCredentials : SecretCredentials
{
    private readonly AppDataFolder sharedAppDataFolder;

    internal SharedFileSecretCredentials(XtiFolder xtiFolder, string key, IDataProtector dataProtector)
        : base(key, dataProtector)
    {
        sharedAppDataFolder = xtiFolder
            .SharedAppDataFolder()
            .WithSubFolder("Secrets");
    }

    protected override void _Delete(string key) => File.Delete(getFilePath(key));

    protected override bool _Exist(string key)
    {
        var filePath = getFilePath(key);
        return File.Exists(filePath);
    }

    protected override Task<string> Load(string key) => LoadShared(key);

    internal async Task<string> LoadShared(string key)
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
            throw new ArgumentException($"Secrets file for '{key}' was not found");
        }
        return text;
    }

    protected override async Task Persist(string key, string encryptedText)
    {
        sharedAppDataFolder.TryCreate();
        using (var writer = new StreamWriter(getFilePath(key), false))
        {
            await writer.WriteAsync(encryptedText);
        }
    }

    private string getFilePath(string key) => sharedAppDataFolder.FilePath($"{key}.secret");
}