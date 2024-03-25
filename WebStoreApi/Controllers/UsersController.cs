using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using WebStoreApi.DTO;
using WebStoreApi.Repository;

namespace WebStoreApi.Controllers
{
    //[Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _users;

        public UsersController(IUsersRepository users)
        {
            _users = users;
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int? page)
        {
            return Ok(await _users.GetAllUsers(page));  
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            return Ok(await _users.GetUserById(id));
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(UserDTO userDTO,Guid id)
        {
            return Ok(await _users.UpdateUser(userDTO,id));
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            return Ok(await _users.DeleteUser(id));
        }
    }
}
