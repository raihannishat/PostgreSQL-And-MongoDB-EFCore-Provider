namespace EFCoreForPostgreSQLAndMongoDB.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddUserAsync(User user)
    {
        if (user != null)
        {
            await _unitOfWork.GetRepository<User>().AddAsync(user);
            await _unitOfWork.CommitChangesAsync();
        }
    }

    public async Task DeleteUserAsync(string id)
    {
        var existingUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

        if (existingUser != null)
        {
            _unitOfWork.GetRepository<User>().Delete(existingUser);
            await _unitOfWork.CommitChangesAsync();
        }
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _unitOfWork.GetRepository<User>().GetAllAsync();
    }

    public async Task<User> GetUserByIdAsync(string id)
    {
        var existingUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

        return existingUser ?? throw new KeyNotFoundException($"Entity with ID '{id}' not found.");
    }

    public async Task<IEnumerable<User>> SearchUserAsync(Expression<Func<User, bool>> predicate)
    {
        var existingUser = await _unitOfWork.GetRepository<User>().FindAsync(predicate);

        return existingUser ?? throw new KeyNotFoundException($"Entity not found.");
    }

    public async Task UpdateUserAsync(User user)
    {
        var existingUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(user.Id);

        if (existingUser != null)
        {
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Address = user.Address;
            existingUser.Contacts = user.Contacts;
            _unitOfWork.GetRepository<User>().Update(existingUser);
            await _unitOfWork.CommitChangesAsync();
        }
    }

    public async Task<IEnumerable<Phone>> GetUserPhoneNumberByIdAsync(string id)
    {
        var user = await GetUserByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with ID '{id}' not found.");

        return [.. user.Contacts!];
    }

    public async Task<Address> GetUserAddressByIdAsync(string id)
    {
        var user = await GetUserByIdAsync(id)
            ?? throw new KeyNotFoundException($"User profile with ID '{id}' not found.");

        return user.Address!;
    }

    public async Task AddAddressAsync(string id, Address address)
    {
        var existingUser = await GetUserByIdAsync(id);

        if (existingUser != null)
        {
            existingUser.Address = address;
            await _unitOfWork.CommitChangesAsync();
        }
    }

    public async Task AddPhoneNumberAsync(string id, Phone phone)
    {
        var existingUser = await GetUserByIdAsync(id);

        if (existingUser != null)
        {
            existingUser.Contacts!.Add(phone);
            await _unitOfWork.CommitChangesAsync();
        }
    }
}
