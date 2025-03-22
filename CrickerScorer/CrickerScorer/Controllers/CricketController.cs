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
        public IActionResult Index()
        {
            // Pass the total score via ViewBag and the ball log as the model.
            ViewBag.TotalScore = _cricketService.TotalScore;
            ViewBag.Overs = _cricketService.OversString;
            ViewBag.Wickets = _cricketService.WicketCount;

            return View(_cricketService.BallActions);
        }

        // POST: /Cricket/AddBallAction
        [HttpPost]
        public IActionResult AddBallAction(int runs, int extraRuns, int overthrowRuns, string commentary, BallType ballType, WicketType wicket)
        {
            var ballAction = new BallAction
            {
                Runs = runs,
                ExtraRuns = extraRuns,
                OverthrowRuns = overthrowRuns,
                Commentary = commentary,
                BallType = ballType,
                Wicket = wicket,
                Timestamp = DateTime.Now
            };

            _cricketService.AddBallAction(ballAction);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UndoLastBall()
        {
            _cricketService.UndoLastBall();
            return RedirectToAction("Index");
        }
    }
}
