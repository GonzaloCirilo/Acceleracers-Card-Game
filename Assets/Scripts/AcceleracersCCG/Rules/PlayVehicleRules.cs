using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Rules
{
    /// <summary>
    /// Rules for playing a Vehicle card from hand to the first Realm.
    /// </summary>
    public static class PlayVehicleRules
    {
        /// <summary>
        /// Validates whether a player can play a vehicle this turn.
        /// </summary>
        public static string Validate(GameState state, int playerIndex, CardInstance vehicleCard)
        {
            var player = state.GetPlayer(playerIndex);

            if (vehicleCard.Data.CardType != CardType.Vehicle)
                return "Card is not a Vehicle.";

            if (!player.Hand.Contains(vehicleCard.UniqueId))
                return "Vehicle is not in hand.";

            if (player.HasPlayedVehicleThisTurn)
                return "Already played a Vehicle this turn.";

            if (state.CurrentPhase != GamePhaseId.PlayVehicle)
                return "Can only play Vehicles during the Play Vehicle phase.";

            return null;
        }
    }
}
