using BCrypt.Net;
using EntityProject.Model;
using System.Collections.Concurrent;

namespace EntityProject.UserHelper
{
    public static class UserList
    {
        private static readonly ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();

        static UserList()
        {
            InitializeHardcodedUsers();
        }

        private static void InitializeHardcodedUsers()
        {
            AddUser(CreateUser("123", "123", new List<string> { "Admin" }));
            AddUser(CreateUser("jane.smith", "secret123", new List<string> { "User" }));
        }

        private static User CreateUser(string username, string password, List<string> roles)
        {
            return new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Roles = roles
            };
        }

        public static User GetUserByUsername(string username)
        {
            _users.TryGetValue(username, out var user);
            return user;
        }

        public static bool IsUserExist(string username)
        {
            return _users.ContainsKey(username);
        }

        public static void AddUser(User user)
        {
            _users[user.Username] = user;
        }
    }
}
