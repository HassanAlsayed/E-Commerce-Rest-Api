using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using WebStoreApi.Data;
using WebStoreApi.DTO;
using WebStoreApi.Models;
//using WebStoreApi.Models;

namespace WebStoreApi.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly WebStoreDbContext _db;
        private readonly IConfiguration _configuration;
       

        public AccountRepository(WebStoreDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
           
        }

            public string GenerateJwtToken(User user)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtSettings:key"]!));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

                var claims = new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
           
        };

                var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],
                    audience: _configuration["JwtSettings:Audience"],
                    claims: claims,
                   expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                return jwtToken;
            
        }

        public async Task<UserProfileDTO> GetUserById(Guid id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null)
            {
                throw new ArgumentException("user not found");
            }
            var userProfile = new UserProfileDTO()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = DateTime.Now,
                Email = user.Email,
                Phone = user.Phone
            };
            return userProfile;
        }

        public async Task<(UserProfileDTO,string)> Login(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
            {
                throw new ArgumentException("Email or password not valid");
            }

            var passwordHasher = new PasswordHasher<User>();

            var result = passwordHasher.VerifyHashedPassword(new User(), user.Password, password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new ArgumentException("Wrong password");
            }
            var token = GenerateJwtToken(user);
            var userProfile = new UserProfileDTO()
            {
                Email = user.Email,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = DateTime.Now,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role
            };

            return (userProfile,token);
        }

        public async Task<UserProfileDTO> Register(UserDTO userDTO)
        {
            //check if email is used or not
            var EmailCount = await _db.Users.CountAsync(x => x.Email == userDTO.Email);
            if (EmailCount > 0)
            {
                throw new ArgumentException("The user already registered");
            }
            //encrypte the password
            var passwordHasher = new PasswordHasher<User>();
            var encryptedPassword = passwordHasher.HashPassword(new User(), userDTO.Password);
            //create new account
            User user = new User()
            {
                Email = userDTO.Email,
                Password = encryptedPassword,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Address = userDTO.Address,
                Phone = userDTO.Phone ?? "",
                Role = "client",
                CreatedAt = DateTime.Now
            };
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            var jwt = GenerateJwtToken(user);

            UserProfileDTO Userprofile = new UserProfileDTO()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Role = user.Role,
                CreatedAt = DateTime.Now,
                Address = user.Address,
            };
           
            return Userprofile;
        }
        public string SendEmail(EmailData data)
        {
            try
            {
                var smtpSettings = GetSmtpSettings();

                using (MailMessage msg = new MailMessage(smtpSettings.UserName, data.To))
                {
                    msg.Subject = data.Subject;
                    msg.Body = data.Body;
                    msg.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
                    {
                        smtp.EnableSsl = smtpSettings.EnableSsl;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);
                        smtp.Send(msg);
                    }

                    return "Email sent successfully";
                }
            }
            catch (Exception ex)
            {
                return $"Error sending email: {ex.Message}";
            }
        }

        private Email GetSmtpSettings()
        {
            return new Email
            {
                Host = "smtp.gmail.com",
                Port = 587,
                UserName = "hassan19alsayed@gmail.com",
                Password = "pppeuvlvupjxriua",
                EnableSsl = true
            };
        }

        public async Task<string> ForgotPassword(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user is null)
            {
                throw new ArgumentException("User not found");
            }

          
            //create pass reset token
            var token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
            var passReset = new ResetPassword()
            {
                Email = email,
                Token = token,
                CreatedAt = DateTime.Now
            };
            await _db.ResetPassword.AddAsync(passReset);
            await _db.SaveChangesAsync();

            //send the pass reset token by email to user

            string emailSubject = "Password Reset";
            string userName = user.FirstName + " " + user.LastName;
            string emailMessage = "Dear" + " " + userName + "\n" +
                "We recvied your password reset request" + "\n" +
                "Please copy the following token and paste it in the password reset form" + "\n" +
                token + "\n\n" +
                "Best Regards" + "\n";
            var emailData = new EmailData()
            {
                To = email,
                Subject = emailSubject,
                Body = emailMessage
            };
            SendEmail(emailData);
            return "Token send to your email";
        }

        public async Task<string> ResetPassword(string token, string password)
        {
            var userToken = await _db.ResetPassword.FirstOrDefaultAsync(x => x.Token == token);
            if (userToken is null)
            {
                throw new ArgumentException("Wrong or expired token");
            }
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == userToken.Email);
            if (user is null)
            {
                throw new ArgumentException("Wrong or expired token");
            }
            var passwordHasher = new PasswordHasher<User>();
            string encryptedPass = passwordHasher.HashPassword(new User(), password);
            user.Password = encryptedPass;
            _db.ResetPassword.Remove(userToken);
            await _db.SaveChangesAsync();
            return "reset password";

        }
    }
}
