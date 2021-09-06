using System.Threading.Tasks;

namespace XTI_Credentials
{
    public interface ICredentialsAccessor
    {
        Task<CredentialValue> Value();
    }
}
