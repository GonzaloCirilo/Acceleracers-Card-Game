using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Commands.Player
{
    /// <summary>
    /// Equip an AcceleCharger from hand to a vehicle stack. Max 1 per vehicle. Costs AP.
    /// </summary>
    public class EquipAcceleChargerCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int CardUniqueId { get; }
        public int TargetVehicleUniqueId { get; }

        public EquipAcceleChargerCommand(int playerIndex, int cardUniqueId, int targetVehicleUniqueId)
        {
            PlayerIndex = playerIndex;
            CardUniqueId = cardUniqueId;
            TargetVehicleUniqueId = targetVehicleUniqueId;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.Hand.Get(CardUniqueId);
            if (card == null)
                return "Card not in hand.";

            var stack = player.GetVehicleStack(TargetVehicleUniqueId);
            if (stack == null)
                return "Target vehicle not in play.";

            var equipError = EquipRules.ValidateAcceleCharger(card.Data, stack);
            if (equipError != null) return equipError;

            var apError = ActionPointRules.ValidateCost(player, card.Data.APCost);
            if (apError != null) return apError;

            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.Hand.Get(CardUniqueId);
            var stack = player.GetVehicleStack(TargetVehicleUniqueId);

            player.Hand.Remove(CardUniqueId);
            stack.AcceleCharger = card;
            player.AP -= card.Data.APCost;
        }

        public void Undo(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.GetVehicleStack(TargetVehicleUniqueId);
            var card = stack.AcceleCharger;
            if (card != null && card.UniqueId == CardUniqueId)
            {
                stack.AcceleCharger = null;
                player.Hand.Add(card);
                player.AP += card.Data.APCost;
            }
        }
    }
}
