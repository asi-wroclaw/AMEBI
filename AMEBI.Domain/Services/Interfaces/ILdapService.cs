using AMEBI.Domain.Models;

namespace AMEBI.Domain.Services
{
    public interface ILdapService
    {
        User Login(string username, string password);
    }
}