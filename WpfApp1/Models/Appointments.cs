public class Appointment
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int StaffId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; }
    public string Status { get; set; }

    // Навигационные свойства
    public Patient Patient { get; set; }
    public Staff Staff { get; set; }
}