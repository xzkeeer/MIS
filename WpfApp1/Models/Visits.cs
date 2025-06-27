public class Visit
{
    public int VisitId { get; set; }
    public int PatientId { get; set; }
    public int StaffId { get; set; }
    public DateTime VisitDate { get; set; }
    public string Status { get; set; }

    // Навигационные свойства
    public Patient Patient { get; set; }
    public Staff Staff { get; set; }
}