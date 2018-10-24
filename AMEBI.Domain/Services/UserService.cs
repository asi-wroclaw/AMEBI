using System.Threading.Tasks;
using AMEBI.Domain.EF;
using AMEBI.Domain.Model;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using Novell.Directory.Ldap;
using AMEBI.Domain.LDAP;
using Microsoft.Extensions.Options;

namespace AMEBI.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;
        private readonly LdapConfig _config;

        public UserService(DatabaseContext context, IOptions<LdapConfig> config)
        {
            _context = context;
            _config = config.Value;
        }

        public async Task<User> FindAsync(string username)
            => await _context.Users.SingleOrDefaultAsync(x => x.Username == username); 
        
        public async Task AddAsync(string username, string password)
        {
            var user = new User(username, password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public void LoginAsync(string username, string password)
        {
            using (var connection = new LdapConnection())
            {
                connection.ConnectionTimeout = _config.ConnectionTimeout;
                try
                {
                    connection.Connect(_config.LdapHost, LdapConnection.DEFAULT_PORT);
                    connection.Bind(_config.AppServiceDn, _config.AppServicePassword);
                    
                    var searchFilter = string.Format(_config.SearchFilter, username);
                    var result = connection.Search(_config.SearchBase, LdapConnection.SCOPE_SUB, searchFilter, new string[] { }, false);

                    var user = result.next();
                    if (user != null)
                    {
                        connection.Bind(user.DN, password);
                        if (connection.Bound)
                        {
                            var loggedUser = new User
                            {
                                DisplayName = $"{user.getAttribute(_config.FirstNameAttribute).StringValue} {user.getAttribute(_config.LastNameAttribute).StringValue}",
                                Username = user.getAttribute(_config.UsernameAttribute).StringValue
                            };

                            Console.WriteLine($"Logged as {loggedUser.Username} ({loggedUser.DisplayName})");
                            connection.Disconnect();
                        }
                    }
                }
                catch (LdapException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}