using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Dapper;

public class StaffService : BaseRepository, IStaffService
{
    public async Task<List<Staff>> GetAllStaffAsync()
    {
        try
        {
            var query = @"SELECT s.*, u.first_name, u.last_name 
                        FROM staff s 
                        JOIN users u ON s.user_id = u.user_id";
            return (await QueryAsync<Staff>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting staff: {ex.Message}");
            return new List<Staff>();
        }
    }

    public async Task<Staff> GetStaffByIdAsync(int staffId)
    {
        try
        {
            var query = @"SELECT s.*, u.first_name, u.last_name 
                        FROM staff s 
                        JOIN users u ON s.user_id = u.user_id
                        WHERE s.staff_id = @StaffId";
            return await QueryFirstOrDefaultAsync<Staff>(query, new { StaffId = staffId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting staff member: {ex.Message}");
            return null;
        }
    }

    public async Task<int> CreateStaffAsync(Staff staff)
    {
        try
        {
            var query = @"INSERT INTO staff 
                        (user_id, position, department, license_number) 
                        VALUES (@UserId, @Position, @Department, @LicenseNumber)
                        RETURNING staff_id";
            return await ExecuteScalarAsync<int>(query, staff);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating staff: {ex.Message}");
            return -1;
        }
    }

    public async Task<bool> UpdateStaffAsync(Staff staff)
    {
        try
        {
            var query = @"UPDATE staff SET 
                        position = @Position,
                        department = @Department,
                        license_number = @LicenseNumber
                        WHERE staff_id = @StaffId";

            var affected = await ExecuteAsync(query, staff);
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating staff: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteStaffAsync(int staffId)
    {
        try
        {
            var affected = await ExecuteAsync(
                "DELETE FROM staff WHERE staff_id = @StaffId",
                new { StaffId = staffId });
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting staff: {ex.Message}");
            return false;
        }
    }
}