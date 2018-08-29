using System;

namespace AMEBI.Domain.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Username { get; internal set; }
        public bool IsAdmin { get; set; }
        
        public User(string username, string password)
        {
            Id = Guid.NewGuid();
            Username = username;
            Password = password; 
        }

        public User() {}
    }
}