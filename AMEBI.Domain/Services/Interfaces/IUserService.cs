using System.Threading.Tasks;
using AMEBI.Domain.Models;

namespace AMEBI.Domain.Services
{
    public interface IUserService
    {
        Task<User> FindAsync(string username);
        Task AddAsync(string username, string password);
        User Login(string username, string password);
    }
}