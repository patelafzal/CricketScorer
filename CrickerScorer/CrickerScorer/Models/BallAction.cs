using CrickerScorer.Enum;

namespace CrickerScorer.Models
{
    public class BallAction
    {
        // Sequential delivery count (increments on every ball attempted)
        public int DeliveryNumber { get; set; }

        // Legal ball number (only increments if the delivery is legal)
        public int? LegalBallNumber { get; set; }

        // Runs scored on the ball (e.g., runs taken by the batsman)
        public int Runs { get; set; }

        // Additional runs (for extras like no ball/wide extra run, etc.)
        public int ExtraRuns { get; set; }

        // Overthrow runs, if any
        public int OverthrowRuns { get; set; }

        // Type of the ball delivery
        public BallType BallType { get; set; }
        public WicketType Wicket { get; set; } = WicketType.None;

        // Optional commentary
        public string? Commentary { get; set; }

        // Timestamp of the delivery
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? ShortCode { get; set; }
        public string? ColorClass { get; set; }

    }

}
