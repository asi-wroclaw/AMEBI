using System.Threading.Tasks;
using AMEBI.Domain.Model;

namespace AMEBI.Domain.Services
{
    public interface IUserService
    {
        Task<User> FindAsync(string username);
        Task AddAsync(string username, string password);
        void LoginAsync(string username, string password);
        Task LogoutAsync();
    }
}