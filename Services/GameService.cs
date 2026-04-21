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

            var unplayedAlivePlayers = _currentSession.Players.Where(p => !p.HasParticipated && !p.IsEliminated).ToList();
            if (unplayedAlivePlayers.Any())
            {
                var random = new Random();
                var nextPlayer = unplayedAlivePlayers[random.Next(unplayedAlivePlayers.Count)];
                _currentSession.CurrentPlayerId = nextPlayer.Id;
            }
            else
            {
                // Todos los vivos ya jugaron su turno.
                _currentSession.Stopwatch.Stop();
                
                if (_currentSession.Mode == GameMode.Collaborative)
                {
                    _currentSession.State = GameState.Won;
                }
                else
                {
                    // Modo Eliminación
                    var alivePlayers = _currentSession.Players.Count(p => !p.IsEliminated);
                    if (alivePlayers <= 1)
                    {
                        var winner = _currentSession.Players.FirstOrDefault(p => !p.IsEliminated);
                        _currentSession.CurrentPlayerId = winner?.Id;
                        _currentSession.State = GameState.Won;
                    }
                    else
                    {
                        _currentSession.State = GameState.RoundBreak;
                    }
                }
            }
        }

        public void StartNextRound()
        {
            if (_currentSession == null || _currentSession.State != GameState.RoundBreak) return;

            foreach (var p in _currentSession.Players.Where(p => !p.IsEliminated))
            {
                p.HasParticipated = false;
                p.WordAnswered = string.Empty; // opcional: limpiar respuesta
            }

            _currentSession.CurrentRound++;
            _currentSession.State = GameState.Playing;
            _currentSession.Stopwatch.Start();

            PickNextPlayer();
        }

        public string? SubmitTurn(string word)
        {
            if (_currentSession == null || _currentSession.State != GameState.Playing) return null;

            word = word.Trim().ToLower();

            if (string.IsNullOrEmpty(word)) return null;

            var currentPlayer = _currentSession.Players.FirstOrDefault(p => p.Id == _currentSession.CurrentPlayerId);

            bool isValidColor = _validColors.Contains(word);
            bool isRepeated = _currentSession.UsedWords.Contains(word);

            string? alertMsg = null;

            if (!isValidColor || isRepeated)
            {
                if (_currentSession.Mode == GameMode.Collaborative)
                {
                    _currentSession.State = GameState.Lost;
                    _currentSession.Stopwatch.Stop();
                    _currentSession.LosingPlayerName = currentPlayer?.Name ?? "General";
                    if (currentPlayer != null) currentPlayer.WordAnswered = isValidColor ? word + " (Repetido)" : word + " (Color Inválido)";
                }
                else
                {
                    // Modo Eliminación
                    if (currentPlayer != null)
                    {
                        currentPlayer.IsEliminated = true;
                        currentPlayer.WordAnswered = isValidColor ? word + " (Repetido)" : word + " (Color Inválido)";
                        alertMsg = $"¡{currentPlayer.Name} fue eliminado por decir un color {(isValidColor ? "repetido" : "inválido")}!";
                    }
                    
                    var alivePlayers = _currentSession.Players.Count(p => !p.IsEliminated);
                    if (alivePlayers <= 1)
                    {
                        _currentSession.Stopwatch.Stop();
                        _currentSession.State = GameState.Won;
                        var winner = _currentSession.Players.FirstOrDefault(p => !p.IsEliminated);
                        _currentSession.CurrentPlayerId = winner?.Id; // Guardar id del ganador absoluto
                    }
                    else
                    {
                        PickNextPlayer();
                    }
                }
                return alertMsg;
            }

            _currentSession.UsedWords.Add(word);
            
            if (currentPlayer != null)
            {
                currentPlayer.HasParticipated = true;
                currentPlayer.WordAnswered = word;
                currentPlayer.AllAnsweredWords.Add(word);
            }
            
            PickNextPlayer();
            return null;
        }
        
        public void ClearSession()
        {
            _currentSession = null;
        }
    }
}
