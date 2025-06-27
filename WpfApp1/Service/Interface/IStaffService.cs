public interface IStaffService
{
    Task<List<Staff>> GetAllStaffAsync();
    Task<Staff> GetStaffByIdAsync(int staffId);
    Task<int> CreateStaffAsync(Staff staff);
    Task<bool> UpdateStaffAsync(Staff staff);
    Task<bool> DeleteStaffAsync(int staffId);
}