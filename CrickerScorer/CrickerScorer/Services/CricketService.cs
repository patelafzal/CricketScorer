using System;
using System.Collections.Generic;
using System.Linq;
using CrickerScorer.Enum;
using CrickerScorer.Models;

namespace CrickerScorer.Services
{
    public class CricketService
    {
        // In-memory list of ball actions.
        public List<BallAction> BallActions { get; } = new List<BallAction>();

        // Internal counters.
        private int _deliveryCount = 0;
        private int _legalDeliveryCount = 0;
        private int _wicketCount = 0;

        // Publicly accessible wicket count.
        public int WicketCount => _wicketCount;

        // Adds a new ball action.
        public void AddBallAction(BallAction action)
        {
            // Increment overall delivery count.
            _deliveryCount++;
            action.DeliveryNumber = _deliveryCount;

            // Increment legal ball count only if the ball type is Legal.
            if (action.BallType == BallType.Legal)
            {
                _legalDeliveryCount++;
                action.LegalBallNumber = _legalDeliveryCount;
            }
            else
            {
                // For illegal deliveries (NoBall, WideBall, DeadBall), legal ball count remains unchanged.
                action.LegalBallNumber = _legalDeliveryCount;
            }

            // If a wicket is recorded on this delivery (via the separate wicket property), increment wicket count.
            if (action.Wicket != WicketType.None)
            {
                _wicketCount++;
            }

            // Force at least +1 extra run if it's a NoBall or WideBall.
            if (action.BallType == BallType.NoBall || action.BallType == BallType.WideBall)
            {
                if (action.ExtraRuns < 1)
                {
                    action.ExtraRuns = 1;
                }
            }

            // Build the short code (e.g. "6N", "4WD", "W", ".", etc.)
            action.ShortCode = BuildShortCode(action);

            // Determine the Tailwind color class (background) based on the short code.
            action.ColorClass = BuildColorClass(action.ShortCode);

            // Finally, add the action to the in-memory list.
            BallActions.Add(action);
        }

        // Undo last ball.
        public void UndoLastBall()
        {
            if (BallActions.Any())
            {
                var lastAction = BallActions.Last();
                BallActions.Remove(lastAction);
                _deliveryCount--;

                // Decrement legal ball count if the delivery was legal.
                if (lastAction.BallType == BallType.Legal && _legalDeliveryCount > 0)
                {
                    _legalDeliveryCount--;
                }

                // If a wicket was recorded on the last ball, decrement wicket count.
                if (lastAction.Wicket != WicketType.None && _wicketCount > 0)
                {
                    _wicketCount--;
                }
            }
        }

        // Total score: sum of runs, extra runs, and overthrow runs.
        public int TotalScore => BallActions.Sum(b => b.Runs + b.ExtraRuns + b.OverthrowRuns);

        // Count of legal deliveries.
        public int LegalBallCount => BallActions.Count(b => b.BallType == BallType.Legal);

        // e.g. "4.2" overs.
        public string OversString
        {
            get
            {
                int overs = LegalBallCount / 6;
                int balls = LegalBallCount % 6;
                return $"{overs}.{balls}";
            }
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
                switch (action.Wicket)
                {
                    case WicketType.Bowled:
                        wicketSuffix = "W";
                        break;
                    case WicketType.Caught:
                        wicketSuffix = "W";
                        break;
                    case WicketType.RunOut:
                        wicketSuffix = action.Runs > 0 ? $"{action.Runs}W" : "W";
                        break;
                    case WicketType.Stumped:
                        wicketSuffix = "W";
                        break;
                    case WicketType.LBW:
                        wicketSuffix = "W";
                        break;
                    case WicketType.HitWicket:
                        wicketSuffix = "W";
                        break;
                    default:
                        wicketSuffix = "W";
                        break;
                }
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
                // Dead ball: not counted as a legal delivery.
                return total == 0 ? "." : total.ToString();
            }

            // For Legal deliveries.
            if (action.BallType == BallType.Legal)
            {
                if (action.Wicket != WicketType.None)
                {
                    // If wicket on legal ball, show run count if any, otherwise just wicket code.
                    return action.Runs > 0 ? $"{action.Runs}{wicketSuffix}" : wicketSuffix;
                }
                else
                {
                    return total == 0 ? "." : total.ToString();
                }
            }

            // Fallback.
            return total.ToString();
        }

        /// <summary>
        /// Assigns a Tailwind color class based on the short code.
        /// </summary>
        private string BuildColorClass(string shortCode)
        {
            // Dot ball or "0".
            if (shortCode == "." || shortCode == "0")
            {
                return "bg-gray-400";
            }
            // Wicket: if the short code contains a wicket indicator, mark it as a wicket ball.
            if (shortCode.Contains("W"))
            {
                return "bg-red-600";
            }
            // No ball.
            if (shortCode.Contains("N"))
            {
                return "bg-orange-400";
            }
            // Wide ball.
            if (shortCode.Contains("WD"))
            {
                return "bg-orange-400";
            }
            // Pure numeric run(s).
            if (int.TryParse(shortCode, out _))
            {
                return "bg-blue-500";
            }
            // Fallback color.
            return "bg-green-500";
        }
    }
}
