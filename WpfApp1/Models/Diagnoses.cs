public class Diagnosis
{
    public int DiagnosisId { get; set; }
    public int PatientId { get; set; }
    public int StaffId { get; set; }
    public string DiagnosisText { get; set; }
    public DateTime DiagnosisDate { get; set; }
    public string Status { get; set; }

    // Навигационные свойства
    public Patient Patient { get; set; }
    public Staff Staff { get; set; }
}