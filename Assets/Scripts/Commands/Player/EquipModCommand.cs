using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Commands.Player
{
    /// <summary>
    /// Equip a Mod from hand to a vehicle stack. Costs AP. Validates modability.
    /// </summary>
    public class EquipModCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int CardUniqueId { get; }
        public int TargetVehicleUniqueId { get; }

        public EquipModCommand(int playerIndex, int cardUniqueId, int targetVehicleUniqueId)
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

            var equipError = EquipRules.ValidateMod(card.Data, stack, state.RealmTrack);
            if (equipError != null) return equipError;

            var apError = ActionPointRules.ValidateCost(player, ((Cards.IAPCostCard)card.Data).APCost);
            if (apError != null) return apError;

            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.Hand.Get(CardUniqueId);
            var stack = player.GetVehicleStack(TargetVehicleUniqueId);

            player.Hand.Remove(CardUniqueId);
            stack.EquippedMods.Add(card);
            player.AP -= ((Cards.IAPCostCard)card.Data).APCost;
        }

        public void Undo(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.GetVehicleStack(TargetVehicleUniqueId);
            var card = stack.RemoveEquipment(CardUniqueId);
            if (card != null)
            {
                player.Hand.Add(card);
                player.AP += ((Cards.IAPCostCard)card.Data).APCost;
            }
        }
    }
}
