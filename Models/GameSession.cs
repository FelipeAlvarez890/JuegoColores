using System.Diagnostics;

namespace JuegoColores.Models
{
    public enum GameMode
    {
        Collaborative,
        Elimination
    }

    public enum GameState
    {
        Setup,
        Playing,
        RoundBreak,
        Won,
        Lost
    }

    public class GameSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Topic { get; set; } = string.Empty;
        public GameMode Mode { get; set; } = GameMode.Collaborative;
        public int CurrentRound { get; set; } = 1;
        public string? LeaderboardEntryId { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public GameState State { get; set; } = GameState.Setup;
        public string? CurrentPlayerId { get; set; }
        
        // Temporizador para la ronda
        public Stopwatch Stopwatch { get; set; } = new Stopwatch();

        // Para evitar repeticiones
        public List<string> UsedWords { get; set; } = new List<string>();

        // Jugador que causó la derrota
        public string? LosingPlayerName { get; set; }
    }
}
