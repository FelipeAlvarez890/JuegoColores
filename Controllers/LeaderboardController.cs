using JuegoColores.Services;
using Microsoft.AspNetCore.Mvc;

namespace JuegoColores.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly LeaderboardService _leaderboardService;

        public LeaderboardController(LeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        public IActionResult Index()
        {
            var entries = _leaderboardService.GetEntries()
                .OrderByDescending(e => e.Result == "Won")
                .ThenBy(e => e.TotalTimeSeconds)
                .ToList();
            
            return View(entries);
        }
    }
}
