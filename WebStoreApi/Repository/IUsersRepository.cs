using WebStoreApi.DTO;

namespace WebStoreApi.Repository
{
    public interface IUsersRepository
    {
        public Task<IEnumerable<UserProfileDTO>> GetAllUsers(int? page);
        public Task<UserProfileDTO> GetUserById(Guid id);
        public Task<UserProfileDTO> UpdateUser(UserDTO user,Guid id);
        public Task<UserProfileDTO> DeleteUser(Guid id);
    }
}
