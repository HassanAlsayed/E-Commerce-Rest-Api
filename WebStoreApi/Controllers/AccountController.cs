using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebStoreApi.DTO;
using WebStoreApi.Models;
using WebStoreApi.Repository;

namespace WebStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _account;
       

        public AccountController(IAccountRepository account)
        {
            _account = account;
           
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            var user = await _account.Register(userDTO);
            return CreatedAtAction(nameof(Register),new {id = user.Id},user);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email,string password)
        {
            var (userProfile, token) = await _account.Login(email, password);
            return Ok(new { UserProfile = userProfile, Token = token });
        }

        [Authorize]
        [HttpGet("ViewProfile")]
        public async Task<IActionResult> ViewProfile()
        {
            var identity = User.Identity as ClaimsIdentity;
            if(identity is null) { 
             return Unauthorized();
            }
            var claim = identity.Claims.FirstOrDefault(c => c.Type.ToLower() == "id");
            if(claim is null)
            {
                return Unauthorized();
            }
            Guid id;
            try
            {
                id = Guid.Parse(claim.Value);
            }
            catch (Exception ex)
            {
                return Unauthorized();  
            }
            var user = await _account.GetUserById(id);
            return Ok(user);
        }

        [Authorize]
        [HttpPost("SendEmail")]
        public IActionResult SendEmail([FromBody] EmailData data)
        {
            return Ok( _account.SendEmail(data));
        }

        [Authorize]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            return Ok(await _account.ForgotPassword(email));
        }
        [Authorize]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string token,string password)
        {
            return Ok(await _account.ResetPassword(token,password));
        }

    }
}
