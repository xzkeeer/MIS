public class PatientInsurance
{
    public int PolicyId { get; set; }
    public int PatientId { get; set; }
    public int InsuranceId { get; set; }
    public string PolicyNumber { get; set; }

    // Навигационные свойства
    public Patient Patient { get; set; }
    public InsuranceCompany InsuranceCompany { get; set; }
}