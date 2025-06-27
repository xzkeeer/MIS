public class Equipment
{
    public int EquipmentId { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public DateTime LastMaintenanceDate { get; set; }
    public int RoomId { get; set; }

    // Навигационное свойство
    public Room Room { get; set; }
}