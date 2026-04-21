namespace JuegoColores.Models
{
    public class Player
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public bool HasParticipated { get; set; } = false;
        public bool IsEliminated { get; set; } = false;
        public string WordAnswered { get; set; } = string.Empty;
    }
}
