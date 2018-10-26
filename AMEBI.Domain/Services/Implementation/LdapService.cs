using System;
using AMEBI.Domain.Config;
using AMEBI.Domain.Models;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace AMEBI.Domain.Services
{
    public class LdapService : ILdapService
    {
        private readonly LdapConfig _config;

        public LdapService(IOptions<LdapConfig> config)
        {
            _config = config.Value;
        }

        public User Login(string username, string password)
        {
            User loggedUser = null;
            using (var connection = new LdapConnection())
            {
                connection.ConnectionTimeout = _config.ConnectionTimeout;
                try
                {
                    connection.Connect(_config.LdapHost, LdapConnection.DEFAULT_PORT);
                    connection.Bind(_config.AppServiceDn, _config.AppServicePassword);

                    LdapEntry userLdapEntry = FindUser(username, connection);

                    if (userLdapEntry != null)
                    {
                        loggedUser = BindUser(connection, userLdapEntry, password);
                    }
                    connection.Disconnect();
                }
                catch (LdapException ex)
                {
                    Console.WriteLine(ex);
                    if (connection.Connected)
                    {
                        connection.Disconnect();
                    }
                }
            }
            return loggedUser;
        }

        private LdapEntry FindUser(string username, LdapConnection connection)
        {
            var searchFilter = string.Format(_config.SearchFilter, username);
            var result = connection.Search(_config.SearchBase, LdapConnection.SCOPE_SUB, searchFilter, new string[] { }, false);
            var userLdapEntry = result.next();
            return userLdapEntry;
        }

        private User BindUser(LdapConnection connection, LdapEntry userLdapEntry, string password)
        {
            connection.Bind(userLdapEntry.DN, password);
            if (connection.Bound)
            {
                var loggedUser = new User
                {
                    DisplayName = $"{userLdapEntry.getAttribute(_config.FirstNameAttribute).StringValue} {userLdapEntry.getAttribute(_config.LastNameAttribute).StringValue}",
                    Username = userLdapEntry.getAttribute(_config.UsernameAttribute).StringValue
                };
                return loggedUser;
            }
            return null;
        }
    }
}