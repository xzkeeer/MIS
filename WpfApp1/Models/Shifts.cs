public class Shift
{
    public int ShiftId { get; set; }
    public int StaffId { get; set; }
    public DateTime ShiftDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    // Навигационное свойство
    public Staff Staff { get; set; }
}