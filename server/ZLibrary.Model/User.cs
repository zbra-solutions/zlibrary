namespace ZLibrary.Model
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsAdministrator { get; set; }
        public string AccessToken { get; set; }
        public string UserAvatarUrl { get; set;}
    }
}
