using JuegoColores.Models;
using JuegoColores.Models.ViewModels;
using JuegoColores.Services;
using Microsoft.AspNetCore.Mvc;

namespace JuegoColores.Controllers
{
    public class GameController : Controller
    {
        private readonly GameService _gameService;
        private readonly LeaderboardService _leaderboardService;

        public GameController(GameService gameService, LeaderboardService leaderboardService)
        {
            _gameService = gameService;
            _leaderboardService = leaderboardService;
        }

        [HttpGet]
        public IActionResult Setup()
        {
            return View(new SetupViewModel());
        }

        [HttpPost]
        public IActionResult Setup(SetupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var validNames = model.PlayerNames.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim()).ToList();
                
                if (validNames.Count < 2)
                {
                    ModelState.AddModelError("", "Se requieren al menos 2 jugadores.");
                    return View(model);
                }

                var distinctNamesCount = validNames.Select(n => n.ToLower()).Distinct().Count();
                if (distinctNamesCount < validNames.Count)
                {
                    ModelState.AddModelError("", "Los nombres de los jugadores no pueden estar repetidos.");
                    return View(model);
                }

                var session = new GameSession
                {
                    Topic = "Colores",
                    Mode = model.SelectedMode,
                    Players = validNames.Select(n => new Player { Name = n }).ToList()
                };

                _gameService.StartNewSession(session);
                return RedirectToAction("Play");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Play()
        {
            var session = _gameService.GetCurrentSession();
            if (session == null) return RedirectToAction("Setup");

            if (session.State == GameState.Won || session.State == GameState.Lost)
            {
                return RedirectToAction("Result");
            }

            return View(session);
        }

        [HttpPost]
        public IActionResult SubmitTurn(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                ModelState.AddModelError("", "Debes escribir la respuesta.");
                return RedirectToAction("Play");
            }

            var alert = _gameService.SubmitTurn(word);
            if (!string.IsNullOrEmpty(alert))
            {
                TempData["AlertMessage"] = alert;
            }

            var session = _gameService.GetCurrentSession();
            if (session?.State == GameState.Won || session?.State == GameState.Lost)
            {
                // Guardar en leaderboard
                var entry = new LeaderboardEntry
                {
                    Topic = session.Topic,
                    PlayerCount = session.Players.Count,
                    TotalTimeSeconds = session.Stopwatch.Elapsed.TotalSeconds,
                    Result = session.State.ToString()
                };
                _leaderboardService.AddEntry(entry);

                return RedirectToAction("Result");
            }

            return RedirectToAction("Play");
        }

        [HttpGet]
        public IActionResult Result()
        {
            var session = _gameService.GetCurrentSession();
            if (session == null) return RedirectToAction("Setup");

            return View(session);
        }

        [HttpPost]
        public IActionResult StartNextRound()
        {
            _gameService.StartNextRound();
            return RedirectToAction("Play");
        }

        [HttpPost]
        public IActionResult EndSession()
        {
            _gameService.ClearSession();
            return RedirectToAction("Index", "Home");
        }
    }
}
