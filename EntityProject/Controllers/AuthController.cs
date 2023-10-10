
using EntityProject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EntityProject.UserHelper;
using EntityProject.Dto;

namespace EntityProject.Controllers
{
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        // Constructor to initialize configuration settings.
        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        // Endpoint to register a new user.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            // Check if the username already exists in the UserList.
            if (UserList.IsUserExist(request.Username))
            {
                return BadRequest("Username is already taken.");
            }

            // Hash the provided password.
            string passwordHash = await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(request.Password));

            // Create a new user object.
            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };

            // Add the new user to UserList.
            UserList.AddUser(newUser);

            return Created("", newUser);
        }

        // Endpoint to authenticate a user and generate a JWT token.
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            // Retrieve the user by username using UserList method.
            var user = UserList.GetUserByUsername(request.Username);

            // If user does not exist, return an error.
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // Verify the provided password with the hashed password in the user object.
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Wrong Password");
            }

            // Create JWT token for the authenticated user.
            string token = CreateToken(user);
            return Ok(new { token });
        }

        // Helper method to create JWT token using user details.
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty)
            };

            // Ensure every user gets the "User" role by default
            if (!user.Roles.Contains("User"))
            {
                user.Roles.Add("User");
            }

            // Add the roles from the user's Roles property to the token claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
