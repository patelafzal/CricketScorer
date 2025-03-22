using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrickerScorer.Enum;
using CrickerScorer.Models;
using CrickerScorer.Data;
using Microsoft.EntityFrameworkCore;

namespace CrickerScorer.Services
{
    public class CricketService
    {
        private readonly CricketDbContext _context;

        public CricketService(CricketDbContext context)
        {
            _context = context;
        }

        public async Task<List<BallAction>> GetAllBallActionsAsync()
        {
            return await _context.BallActions.OrderBy(b => b.DeliveryNumber).ToListAsync();
        }

        public async Task AddBallActionAsync(BallAction action)
        {
            // Get current maximum delivery number (or 0 if none).
            int currentDeliveryCount = await _context.BallActions.MaxAsync(b => (int?)b.DeliveryNumber) ?? 0;
            int currentLegalDeliveryCount = await _context.BallActions.MaxAsync(b => (int?)b.LegalBallNumber) ?? 0;

            action.DeliveryNumber = currentDeliveryCount + 1;
            if (action.BallType == BallType.Legal)
            {
                action.LegalBallNumber = currentLegalDeliveryCount + 1;
            }
            else
            {
                // For illegal deliveries (NoBall, WideBall, DeadBall), legal ball count remains unchanged.
                action.LegalBallNumber = currentLegalDeliveryCount;
            }

            // Force at least +1 extra run if it's a NoBall or WideBall.
            if (action.BallType == BallType.NoBall || action.BallType == BallType.WideBall)
            {
                if (action.ExtraRuns < 1)
                {
                    action.ExtraRuns = 1;
                }
            }

            // Build the short code and color class.
            action.ShortCode = BuildShortCode(action);
            action.ColorClass = BuildColorClass(action.ShortCode);

            _context.BallActions.Add(action);
            await _context.SaveChangesAsync();
        }

        public async Task UndoLastBallAsync()
        {
            var lastAction = await _context.BallActions.OrderByDescending(b => b.DeliveryNumber).FirstOrDefaultAsync();
            if (lastAction != null)
            {
                _context.BallActions.Remove(lastAction);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalScoreAsync()
        {
            return await _context.BallActions.SumAsync(b => b.Runs + b.ExtraRuns + b.OverthrowRuns);
        }

        public async Task<int> GetLegalBallCountAsync()
        {
            return await _context.BallActions.CountAsync(b => b.BallType == BallType.Legal);
        }

        public async Task<string> GetOversStringAsync()
        {
            int legalCount = await GetLegalBallCountAsync();
            int overs = legalCount / 6;
            int balls = legalCount % 6;
            return $"{overs}.{balls}";
        }

        public async Task<int> GetWicketCountAsync()
        {
            return await _context.BallActions.CountAsync(b => b.Wicket != WicketType.None);
        }

        /// <summary>
        /// Builds a short code like "4WD", "6N", "W", ".", etc.
        /// </summary>
        private string BuildShortCode(BallAction action)
        {
            // Total runs from runs + extras + overthrows.
            int total = action.Runs + action.ExtraRuns + action.OverthrowRuns;
            string wicketSuffix = string.Empty;

            // Append wicket information if a wicket is recorded.
            if (action.Wicket != WicketType.None)
            {
                // For simplicity, use a generic "W" – you can expand this to include different abbreviations.
                wicketSuffix = "W";
            }

            // Build short code based on ball type.
            if (action.BallType == BallType.NoBall)
            {
                int displayRuns = total - 1; // Remove the extra run that is automatically added.
                return !string.IsNullOrEmpty(wicketSuffix)
                    ? $"{displayRuns}N{wicketSuffix}"
                    : $"{displayRuns}N";
            }
            if (action.BallType == BallType.WideBall)
            {
                int displayRuns = total - 1;
                return !string.IsNullOrEmpty(wicketSuffix)
                    ? $"{displayRuns}WD{wicketSuffix}"
                    : $"{displayRuns}WD";
            }
            if (action.BallType == BallType.DeadBall)
            {
                return total == 0 ? "." : total.ToString();
            }
            if (action.BallType == BallType.Legal)
            {
                return action.Wicket != WicketType.None
                    ? (action.Runs > 0 ? $"{action.Runs}{wicketSuffix}" : wicketSuffix)
                    : (total == 0 ? "." : total.ToString());
            }
            return total.ToString();
        }

        /// <summary>
        /// Assigns a Tailwind color class based on the short code.
        /// </summary>
        private string BuildColorClass(string shortCode)
        {
            if (shortCode == "." || shortCode == "0")
            {
                return "bg-gray-400";
            }
            if (shortCode.Contains("W"))
            {
                return "bg-red-600";
            }
            if (shortCode.Contains("N"))
            {
                return "bg-orange-400";
            }
            if (shortCode.Contains("WD"))
            {
                return "bg-orange-400";
            }
            if (int.TryParse(shortCode, out _))
            {
                return "bg-blue-500";
            }
            return "bg-green-500";
        }
    }
}
