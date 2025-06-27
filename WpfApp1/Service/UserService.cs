using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Dapper;

public class UserService : BaseRepository, IUserService
{
    public async Task<List<User>> GetAllUsersAsync()
    {
        try
        {
            return (await QueryAsync<User>("SELECT * FROM users")).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting users: {ex.Message}");
            return new List<User>();
        }
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        try
        {
            return await QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM users WHERE user_id = @UserId",
                new { UserId = userId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user: {ex.Message}");
            return null;
        }
    }

    public async Task<int> CreateUserAsync(User user)
    {
        try
        {
            var query = @"INSERT INTO users 
                        (username, email, password_hash, first_name, last_name, role) 
                        VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @Role)
                        RETURNING user_id";
            return await ExecuteScalarAsync<int>(query, user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user: {ex.Message}");
            return -1;
        }
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        try
        {
            var query = @"UPDATE users SET 
                        username = @Username, 
                        email = @Email,
                        first_name = @FirstName,
                        last_name = @LastName,
                        role = @Role
                        WHERE user_id = @UserId";

            var affected = await ExecuteAsync(query, user);
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        try
        {
            var affected = await ExecuteAsync(
                "DELETE FROM users WHERE user_id = @UserId",
                new { UserId = userId });
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user: {ex.Message}");
            return false;
        }
    }

    public async Task<List<User>> GetUsersByRoleAsync(string role)
    {
        try
        {
            return (await QueryAsync<User>(
                "SELECT * FROM users WHERE role = @Role",
                new { Role = role })).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting users by role: {ex.Message}");
            return new List<User>();
        }
    }
}