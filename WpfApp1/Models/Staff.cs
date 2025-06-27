public class Staff
{
    public int StaffId { get; set; }
    public int UserId { get; set; }
    public string Position { get; set; }
    public string Department { get; set; }
    public string LicenseNumber { get; set; }

    // Навигационное свойство
    public User User { get; set; }
}   