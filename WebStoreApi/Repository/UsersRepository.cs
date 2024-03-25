using Microsoft.EntityFrameworkCore;
using WebStoreApi.Data;
using WebStoreApi.DTO;
using WebStoreApi.Models;

namespace WebStoreApi.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly WebStoreDbContext _db;

        public UsersRepository(WebStoreDbContext db)
        {
            _db = db;
        }

        public async Task<UserProfileDTO> DeleteUser(Guid id)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (user is null)
            {
                throw new ArgumentException("User not found");
            }
            var userProfile = new UserProfileDTO()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Address = user.Address,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Role = user.Role
            };
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return userProfile;
        }

        public async Task<IEnumerable<UserProfileDTO>> GetAllUsers(int? page)
        {
            if(page ==  null || page < 1)
            {
                page = 1;
            }
            int pageSize = 5;
            int totalPages = 0;
            decimal totalCount = _db.Users.Count();
            totalPages = (int)Math.Ceiling(totalCount / pageSize);
            var users = await _db.Users.Skip((int)(page -1)*pageSize).Take(pageSize).ToListAsync();
            List<UserProfileDTO> result = new List<UserProfileDTO>();
            foreach (User user in users) {

                var usersProfile = new UserProfileDTO()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt,
                    Email = user.Email,
                    Role = user.Role

                };
                result.Add(usersProfile);
            }
            return result;
            
        }

        public async Task<UserProfileDTO> GetUserById(Guid id)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
            if(user is null)
            {
                throw new ArgumentException("User not fount");
            }
            var userProfile = new UserProfileDTO()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Address = user.Address,
                CreatedAt = user.CreatedAt,
                Email = user.Email,
                Role = user.Role
            };
            return userProfile;
        }

        public async Task<UserProfileDTO> UpdateUser(UserDTO userDTO, Guid id)
        {
           var user = _db.Users.SingleOrDefault(x => x.Id == id);
            if (user is null)
            {
                throw new ArgumentException("User not found");
            }


            user.FirstName = userDTO.FirstName;
            user.LastName = userDTO.LastName;
            user.Address = userDTO.Address;
            user.Phone = userDTO.Phone ?? "";
            user.CreatedAt = DateTime.Now;
            user.Email = userDTO.Email;

            var userProfile = new UserProfileDTO()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = userDTO.Address,
                CreatedAt = DateTime.Now,
                Email = userDTO.Email,
                Role = user.Role,
                Id = user.Id,
                Phone = user.Phone
            };
                
          
            await _db.SaveChangesAsync();   
            return userProfile;
        }
    }
}
