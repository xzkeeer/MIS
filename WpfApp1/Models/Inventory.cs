public class InventoryItem
{
    public int ItemId { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public int Quantity { get; set; }
    public int MinimumStock { get; set; }
    public int RoomId { get; set; }

    // Навигационное свойство
    public Room Room { get; set; }
}