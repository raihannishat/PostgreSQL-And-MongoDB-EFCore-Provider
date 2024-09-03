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
        if (user == null) throw new ArgumentNullException(nameof(user));

        await _unitOfWork.BeginTransactionAsync(); // Start a transaction

        try
        {
            await _unitOfWork.GetRepository<User>().AddAsync(user);
            await _unitOfWork.CommitChangesAsync();
            await _unitOfWork.CommitTransactionAsync(); // Commit the transaction
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(); // Rollback the transaction on failure
            throw new InvalidOperationException("Failed to add user.", ex); // Throw a detailed exception
        }
    }

    public async Task DeleteUserAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("User ID cannot be null or empty.", nameof(id));

        await _unitOfWork.BeginTransactionAsync(); // Start a transaction

        try
        {
            var existingUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

            if (existingUser != null)
            {
                _unitOfWork.GetRepository<User>().Delete(existingUser);
                await _unitOfWork.CommitChangesAsync();
                await _unitOfWork.CommitTransactionAsync(); // Commit the transaction
            }
            else
            {
                throw new KeyNotFoundException($"User with ID '{id}' not found.");
            }
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(); // Rollback the transaction on failure
            throw new InvalidOperationException("Failed to delete user.", ex); // Throw a detailed exception
        }
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _unitOfWork.GetRepository<User>().GetAllAsync();
    }

    public async Task<User> GetUserByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("User ID cannot be null or empty.", nameof(id));

        var existingUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

        return existingUser ?? throw new KeyNotFoundException($"User with ID '{id}' not found.");
    }

    public async Task<IEnumerable<User>> SearchUserAsync(Expression<Func<User, bool>> predicate)
    {
        var users = await _unitOfWork.GetRepository<User>().FindAsync(predicate);

        return users ?? throw new KeyNotFoundException("No users found matching the criteria.");
    }

    public async Task UpdateUserAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        await _unitOfWork.BeginTransactionAsync(); // Start a transaction

        try
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
                await _unitOfWork.CommitTransactionAsync(); // Commit the transaction
            }
            else
            {
                throw new KeyNotFoundException($"User with ID '{user.Id}' not found.");
            }
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(); // Rollback the transaction on failure
            throw new InvalidOperationException("Failed to update user.", ex); // Throw a detailed exception
        }
    }

    public async Task<IEnumerable<Phone>> GetUserPhoneNumberByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("User ID cannot be null or empty.", nameof(id));

        var user = await GetUserByIdAsync(id);

        return user.Contacts ?? Enumerable.Empty<Phone>(); // Return empty if Contacts is null
    }

    public async Task<Address> GetUserAddressByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("User ID cannot be null or empty.", nameof(id));

        var user = await GetUserByIdAsync(id);

        return user.Address ?? throw new KeyNotFoundException($"Address for user with ID '{id}' not found.");
    }

    public async Task AddAddressAsync(string id, Address address)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("User ID cannot be null or empty.", nameof(id));
        ArgumentNullException.ThrowIfNull(address);

        await _unitOfWork.BeginTransactionAsync(); // Start a transaction

        try
        {
            var existingUser = await GetUserByIdAsync(id);

            if (existingUser != null)
            {
                existingUser.Address = address;
                await _unitOfWork.CommitChangesAsync();
                await _unitOfWork.CommitTransactionAsync(); // Commit the transaction
            }
            else
            {
                throw new KeyNotFoundException($"User with ID '{id}' not found.");
            }
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(); // Rollback the transaction on failure
            throw new InvalidOperationException("Failed to add address.", ex); // Throw a detailed exception
        }
    }

    public async Task AddPhoneNumberAsync(string id, Phone phone)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(id));
        }

        ArgumentNullException.ThrowIfNull(phone);

        await _unitOfWork.BeginTransactionAsync(); // Start a transaction

        try
        {
            var existingUser = await GetUserByIdAsync(id);

            if (existingUser != null)
            {
                existingUser.Contacts ??= new List<Phone>(); // Initialize Contacts if null
                existingUser.Contacts.Add(phone);
                await _unitOfWork.CommitChangesAsync();
                await _unitOfWork.CommitTransactionAsync(); // Commit the transaction
            }
            else
            {
                throw new KeyNotFoundException($"User with ID '{id}' not found.");
            }
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(); // Rollback the transaction on failure
            throw new InvalidOperationException("Failed to add phone number.", ex); // Throw a detailed exception
        }
    }
}