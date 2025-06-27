public class Message
{
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public string Text { get; set; }
    public DateTime SendDate { get; set; }

    // Навигационные свойства
    public User Sender { get; set; }
    public User Recipient { get; set; }
}