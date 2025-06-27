public class Prescription
{
    public int PrescriptionId { get; set; }
    public int PatientId { get; set; }
    public int StaffId { get; set; }
    public string PrescriptionType { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Навигационные свойства
    public Patient Patient { get; set; }
    public Staff Staff { get; set; }
}