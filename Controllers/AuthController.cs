using Auth.Data;
using Auth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Auth.Utils;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        public IConfiguration _config;
        public AuthController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto req)
        {
            var existingUser = await _context.Users.Where(u => u.Username == req.Username).FirstOrDefaultAsync();
            if(existingUser != null)
            {
                return BadRequest("User already exists");
            }

            PasswordTools.CreatePasswordHash(req.Password, out byte[] passwordSalt, out byte[] passwordHash);

            User user = new User();
            if(req.Base64PhotoString == null)
            {
                user.Photo = null;
            }
            else
            {
                byte[] ReqPhotoByteArr = Convert.FromBase64String(req.Base64PhotoString);
                user.Photo = ReqPhotoByteArr;
            }

            user.Username = req.Username;
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto req)
        {
            var user = await _context.Users.Where(u => u.Username == req.Username).FirstOrDefaultAsync();
            if(user == null)
            {
                return BadRequest("User doesn't exist");
            }
            if(user.Username != req.Username)
            {
                return BadRequest("User not found");
            }
            if(PasswordTools.VerifyHash(req.Password, user.PasswordSalt, user.PasswordHash))
            {
                return BadRequest("Wrong password");
            }

            string token = JwtTools.CreateToken(user, _config);
            return Ok(token);
        }
    }
}
