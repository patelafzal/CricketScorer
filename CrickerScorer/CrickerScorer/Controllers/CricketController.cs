using CrickerScorer.Enum;
using CrickerScorer.Models;
using CrickerScorer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace CrickerScorer.Controllers
{
    public class CricketController : Controller
    {
        private readonly CricketService _cricketService;

        public CricketController(CricketService cricketService)
        {
            _cricketService = cricketService;
        }

        // GET: /Cricket/Index
        public async Task<IActionResult> Index()
        {
            // Pass the total score via ViewBag and the ball log as the model.
            ViewBag.TotalScore = await _cricketService.GetTotalScoreAsync();
            ViewBag.Overs = await _cricketService.GetOversStringAsync();
            ViewBag.Wickets = await _cricketService.GetWicketCountAsync();

            return View(await _cricketService.GetAllBallActionsAsync());
        }

        // POST: /Cricket/AddBallAction
        [HttpPost]
        public async Task<IActionResult> AddBallAction(int runs, int extraRuns, int overthrowRuns, string commentary, BallType ballType, WicketType wicket)
        {
            var ballAction = new BallAction
            {
                Runs = runs,
                ExtraRuns = extraRuns,
                OverthrowRuns = overthrowRuns,
                Commentary = commentary,
                BallType = ballType,
                Wicket = wicket,
                Timestamp = DateTime.UtcNow
            };

            await _cricketService.AddBallActionAsync(ballAction);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UndoLastBall()
        {
            await _cricketService.UndoLastBallAsync();
            return RedirectToAction("Index");
        }
    }
}
