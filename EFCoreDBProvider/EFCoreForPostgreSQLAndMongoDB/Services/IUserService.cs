
namespace EFCoreForPostgreSQLAndMongoDB.Services
{
    public interface IUserService
    {
        Task AddAddressAsync(string id, Address address);
        Task AddPhoneNumberAsync(string id, Phone phone);
        Task AddUserAsync(User user);
        Task DeleteUserAsync(string id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<Address> GetUserAddressByIdAsync(string id);
        Task<User> GetUserByIdAsync(string id);
        Task<IEnumerable<Phone>> GetUserPhoneNumberByIdAsync(string id);
        Task<IEnumerable<User>> SearchUserAsync(Expression<Func<User, bool>> predicate);
        Task UpdateUserAsync(User user);
    }
}