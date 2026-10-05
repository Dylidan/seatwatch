public class PriceAlert
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event? Event { get; set; }
    public string Email { get; set; } = "";
    public decimal TargetPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsTriggered { get; set; }
}