namespace AMEBI.Domain.Config
{
    public class LdapConfig
    {
        public string AppServicePassword { get; set; }
        public string FirstNameAttribute { get; set; }
        public string LastNameAttribute { get; set; }
        public string UsernameAttribute { get; set; }
        public string SearchBase { get; set; }
        public string AppServiceDn { get; set; }
        public string LdapHost { get; set; }
        public string SearchFilter { get; set; }
        public int ConnectionTimeout { get; set; }
    }
}