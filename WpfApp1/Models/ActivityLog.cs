public class ActivityLog
{
    public int LogId { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; }
    public DateTime ActionDate { get; set; }

    // Навигационное свойство
    public User User { get; set; }
}