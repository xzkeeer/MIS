public class Clinic
{
    public int ClinicId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string WorkingHours { get; set; }
    public int? ScheduleId { get; set; }

    // Навигационное свойство
    public Schedule Schedule { get; set; }
}