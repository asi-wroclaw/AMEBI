using System.Threading.Tasks;
using AMEBI.Domain.DataAccess;
using AMEBI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using AMEBI.Domain.Config;
using Microsoft.Extensions.Options;

namespace AMEBI.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;
        private readonly ILdapService _ldapService;
        private readonly LdapConfig _config;

        public UserService(DatabaseContext context, ILdapService ldapService, IOptions<LdapConfig> config)
        {
            _context = context;
            _ldapService = ldapService;
            _config = config.Value;
        }

        public async Task<User> FindAsync(string username)
            => await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

        public async Task AddAsync(string username, string password)
        {
            var userLdapEntry = new User(username);

            await _context.Users.AddAsync(userLdapEntry);
            await _context.SaveChangesAsync();
        }

        public User Login(string username, string password)
        {
            return _ldapService.Login(username, password);
        }
    }
}