public class Room
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; }
    public int DepartmentId { get; set; }
    public int? StaffId { get; set; }

    // Навигационные свойства
    public Department Department { get; set; }
    public Staff Staff { get; set; }
}