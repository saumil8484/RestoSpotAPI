using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoSpotAPI.Data;
using RestoSpotAPI.Helpers;
using RestoSpotAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace RestoSpotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RestoSpotDbContext _restoSpotDbContext;
        public UserController(RestoSpotDbContext restoSpotDbContext)
        {
            _restoSpotDbContext = restoSpotDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if(userObj == null)
                return BadRequest();

            var user = await _restoSpotDbContext.Users.FirstOrDefaultAsync(x => x.Username == userObj.Username);
            
            if (user == null)
                return NotFound(new {Message = "User Not Found!"});

            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
                return BadRequest(new { Message = "Password is incorrect!" });

            user.Token = CreateJwt(user);

            return Ok(new { Token = user.Token, Message = "Login Success!" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            if (await CheckUserNameExistAsync(userObj.Username))
                return BadRequest(new { Message = "Username already exists !" });

            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email already exists !" });

            var pass = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString() });

            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "Admin";
            userObj.Token = "";

            await _restoSpotDbContext.Users.AddAsync(userObj);
            await _restoSpotDbContext.SaveChangesAsync();

            return Ok(new { Message = "User Registered!" });
        }


        private Task<bool> CheckUserNameExistAsync(string username)
            => _restoSpotDbContext.Users.AnyAsync(x => x.Username == username);

        private Task<bool> CheckEmailExistAsync(string email)
            => _restoSpotDbContext.Users.AnyAsync(x => x.Email == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if(password.Length < 8)
                sb.Append("Minimum password length should be 8 characters" + Environment.NewLine);

            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be alpha-numeric" + Environment.NewLine);

            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,//,{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special character" + Environment.NewLine);

            return sb.ToString();
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _restoSpotDbContext.Users.ToListAsync());
        }
    }
}
