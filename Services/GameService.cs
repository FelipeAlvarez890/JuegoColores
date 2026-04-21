using JuegoColores.Models;

namespace JuegoColores.Services
{
    public class GameService
    {
        private GameSession? _currentSession;

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

            var currentPlayer = _currentSession.Players.FirstOrDefault(p => p.Id == _currentSession.CurrentPlayerId);

            // Ocurre error si ya se usó la palabra
            if (string.IsNullOrEmpty(word) || _currentSession.UsedWords.Contains(word))
            {
                _currentSession.State = GameState.Lost;
                _currentSession.Stopwatch.Stop();
                _currentSession.LosingPlayerName = currentPlayer?.Name ?? "General";
                if (currentPlayer != null) currentPlayer.WordAnswered = word;
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
