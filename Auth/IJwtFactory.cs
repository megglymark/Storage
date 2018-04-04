using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Storage.Auth
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(string userName, Guid id, Guid ownerId);
    }
}
