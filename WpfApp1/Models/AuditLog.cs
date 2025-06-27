public class AuditLog
{
    public int AuditId { get; set; }
    public string TableName { get; set; }
    public int RecordId { get; set; }
    public string Action { get; set; }
    public int UserId { get; set; }
    public DateTime ActionDate { get; set; }

    // Навигационное свойство
    public User User { get; set; }
}