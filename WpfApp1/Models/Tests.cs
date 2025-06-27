public class Test
{
    public int TestId { get; set; }
    public int PatientId { get; set; }
    public string TestType { get; set; }
    public string Result { get; set; }
    public DateTime TestDate { get; set; }
    public string Status { get; set; }

    // Навигационное свойство
    public Patient Patient { get; set; }
}