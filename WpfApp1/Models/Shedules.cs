public class Schedule
{
    public int ScheduleId { get; set; }
    public int StaffId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }

    // Навигационное свойство
    public Staff Staff { get; set; }
}