public class MedicalHistory
{
    public int HistoryId { get; set; }
    public int PatientId { get; set; }
    public string EventType { get; set; }
    public string Description { get; set; }
    public DateTime EventDate { get; set; }

    // Навигационное свойство
    public Patient Patient { get; set; }
}