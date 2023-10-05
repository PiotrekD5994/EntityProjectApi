using EntityProject.Model;
using System.Collections.Concurrent;

namespace EntityProject.UserHelper
{
    public static class UserList
    {
        // Concurrent dictionary to store users, indexed by their username.
        private static readonly ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();

        // Static constructor to initialize hardcoded users upon class loading.
        static UserList()
        {
            InitializeHardcodedUsers();
        }

        // Initializes hardcoded users into the dictionary.
        private static void InitializeHardcodedUsers()
        {
            AddUser(CreateUser("123", "123"));
            AddUser(CreateUser("jane.smith", "secret123"));
        }

        // Creates a new User object with hashed password.
        private static User CreateUser(string username, string password)
        {
            return new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
        }

        // Retrieves a user by username from the dictionary. Returns null if the user doesn't exist.
        public static User GetUserByUsername(string username)
        {
            _users.TryGetValue(username, out var user);
            return user;
        }

        // Checks if a user with the given username exists in the dictionary.
        public static bool IsUserExist(string username)
        {
            return _users.ContainsKey(username);
        }

        // Adds or updates a user in the dictionary.
        public static void AddUser(User user)
        {
            _users[user.Username] = user;
        }
    }
}
