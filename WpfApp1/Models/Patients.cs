public class Patient
{
    public int PatientId { get; set; }
    public int UserId { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Gender { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string InsurancePolicy { get; set; }

    // Навигационное свойство
    public User User { get; set; }
}