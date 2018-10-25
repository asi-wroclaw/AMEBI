using System;

namespace AMEBI.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; internal set; }

        public User(string username)
        {
            Id = Guid.NewGuid();
            Username = username;
        }

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}