using Microsoft.AspNetCore.Mvc;
using WebStoreApi.DTO;
using WebStoreApi.Models;

namespace WebStoreApi.Repository
{
    public interface IAccountRepository
    {
        public Task<UserProfileDTO> Register(UserDTO userDTO);
        public string GenerateJwtToken(User user);
        public Task<(UserProfileDTO,string)> Login(string email,string password);
        public Task<UserProfileDTO> GetUserById(Guid id);
        public string SendEmail(EmailData data);
        public Task<string> ForgotPassword(string email);
        public Task<string> ResetPassword(string token,string password);

    }
}
