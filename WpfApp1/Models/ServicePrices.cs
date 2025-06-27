public class ServicePrice
{
    public int ServiceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int ClinicId { get; set; }

    // Навигационное свойство
    public Clinic Clinic { get; set; }
}