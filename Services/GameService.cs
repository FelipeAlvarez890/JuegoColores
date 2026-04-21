using JuegoColores.Models;

namespace JuegoColores.Services
{
    public class GameService
    {
        private GameSession? _currentSession;

        private readonly HashSet<string> _validColors = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "rojo", "azul", "verde", "amarillo", "naranja", "morado", "violeta", "blanco", "negro", "gris", 
            "rosa", "rosado", "cafe", "café", "marron", "marrón", "celeste", "turquesa", "cian", "cyan", 
            "magenta", "dorado", "plateado", "coral", "salmon", "salmón", "beige", "purpura", "púrpura",
            "lila", "fucsia", "indigo", "índigo", "ambar", "ámbar", "aguamarina", "crema", "burdeos", 
            "vino", "caqui", "esmeralda", "oliva", "jade", "lavanda", "mostaza", "carmesí", "carmesi", "ocre"
        };

        public GameSession? GetCurrentSession() => _currentSession;

        public void StartNewSession(GameSession session)
        {
            _currentSession = session;
            _currentSession.State = GameState.Playing;
            _currentSession.Stopwatch.Restart();
            PickNextPlayer();
        }

        public void PickNextPlayer()
        {
            if (_currentSession == null) return;

            var unplayedPlayers = _currentSession.Players.Where(p => !p.HasParticipated).ToList();
            if (unplayedPlayers.Any())
            {
                var random = new Random();
                var nextPlayer = unplayedPlayers[random.Next(unplayedPlayers.Count)];
                _currentSession.CurrentPlayerId = nextPlayer.Id;
            }
            else
            {
                // Todos jugaron
                _currentSession.State = GameState.Won;
                _currentSession.Stopwatch.Stop();
            }
        }

        public void SubmitTurn(string word)
        {
            if (_currentSession == null || _currentSession.State != GameState.Playing) return;

            word = word.Trim().ToLower();

            if (string.IsNullOrEmpty(word)) return;

            var currentPlayer = _currentSession.Players.FirstOrDefault(p => p.Id == _currentSession.CurrentPlayerId);

            // Validar si es un color real
            if (!_validColors.Contains(word))
            {
                _currentSession.State = GameState.Lost;
                _currentSession.Stopwatch.Stop();
                _currentSession.LosingPlayerName = currentPlayer?.Name ?? "General";
                if (currentPlayer != null) currentPlayer.WordAnswered = word + " (Color Inválido)";
                return;
            }

            // Ocurre error si ya se usó la palabra
            if (_currentSession.UsedWords.Contains(word))
            {
                _currentSession.State = GameState.Lost;
                _currentSession.Stopwatch.Stop();
                _currentSession.LosingPlayerName = currentPlayer?.Name ?? "General";
                if (currentPlayer != null) currentPlayer.WordAnswered = word + " (Repetido)";
            }
            else
            {
                _currentSession.UsedWords.Add(word);
                
                if (currentPlayer != null)
                {
                    currentPlayer.HasParticipated = true;
                    currentPlayer.WordAnswered = word;
                }
                
                PickNextPlayer();
            }
        }
        
        public void ClearSession()
        {
            _currentSession = null;
        }
    }
}
