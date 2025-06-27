public class Invoice
{
    public int InvoiceId { get; set; }
    public int PatientId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }

    // Навигационное свойство
    public Patient Patient { get; set; }
}