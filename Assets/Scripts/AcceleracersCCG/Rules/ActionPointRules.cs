using AcceleracersCCG.Core;

namespace AcceleracersCCG.Rules
{
    /// <summary>
    /// Action Point cost validation and team bonus calculation.
    /// </summary>
    public static class ActionPointRules
    {
        /// <summary>
        /// Calculate total AP for a player at start of their action phase.
        /// Base 3 + 1 per team that has 2+ vehicles in play.
        /// </summary>
        public static int CalculateAP(PlayerState player)
        {
            int ap = Constants.BaseAP;

            var teamCounts = player.GetTeamCounts();
            foreach (var kvp in teamCounts)
            {
                if (kvp.Value >= 2)
                    ap += 1;
            }

            return ap;
        }

        /// <summary>
        /// Checks if the player can afford a given AP cost.
        /// </summary>
        public static bool CanAfford(PlayerState player, int apCost)
        {
            return player.AP >= apCost;
        }

        /// <summary>
        /// Returns validation message if player can't afford the cost, null if they can.
        /// </summary>
        public static string ValidateCost(PlayerState player, int apCost)
        {
            if (apCost < 0)
                return "AP cost cannot be negative.";
            if (player.AP < apCost)
                return $"Not enough AP. Have {player.AP}, need {apCost}.";
            return null;
        }
    }
}
