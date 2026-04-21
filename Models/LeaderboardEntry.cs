namespace JuegoColores.Models
{
    public class LeaderboardEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Topic { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public double TotalTimeSeconds { get; set; }
        public string Result { get; set; } = "Won"; // "Won" o "Lost"
        public DateTime DatePlayed { get; set; } = DateTime.UtcNow;
    }
}
