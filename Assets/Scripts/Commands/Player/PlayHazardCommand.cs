using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Commands.Player
{
    /// <summary>
    /// Play a Hazard card targeting an opponent's equipment.
    /// Applies SPP damage; if any stat hits 0, the target is junked.
    /// </summary>
    public class PlayHazardCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int HazardCardUniqueId { get; }
        public int TargetPlayerIndex { get; }
        public int TargetVehicleUniqueId { get; }
        public int TargetEquipmentUniqueId { get; }

        // Undo state
        private SPP _originalTargetSPP;
        private bool _wasJunked;

        public PlayHazardCommand(int playerIndex, int hazardCardUniqueId,
            int targetPlayerIndex, int targetVehicleUniqueId, int targetEquipmentUniqueId)
        {
            PlayerIndex = playerIndex;
            HazardCardUniqueId = hazardCardUniqueId;
            TargetPlayerIndex = targetPlayerIndex;
            TargetVehicleUniqueId = targetVehicleUniqueId;
            TargetEquipmentUniqueId = targetEquipmentUniqueId;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var hazardCard = player.Hand.Get(HazardCardUniqueId);
            if (hazardCard == null)
                return "Hazard card not in hand.";

            if (hazardCard.Data.CardType != CardType.Hazard)
                return "Card is not a Hazard.";

            var apError = ActionPointRules.ValidateCost(player, ((Cards.IAPCostCard)hazardCard.Data).APCost);
            if (apError != null) return apError;

            var targetPlayer = state.GetPlayer(TargetPlayerIndex);
            var targetStack = targetPlayer.GetVehicleStack(TargetVehicleUniqueId);
            if (targetStack == null)
                return "Target vehicle not in play.";

            var targetEquip = targetStack.FindEquipment(TargetEquipmentUniqueId);
            if (targetEquip == null)
                return "Target equipment not found on vehicle.";

            var targetError = HazardTargetRules.ValidateTarget(hazardCard.Data, targetEquip);
            if (targetError != null) return targetError;

            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var hazardCard = player.Hand.Get(HazardCardUniqueId);

            // Remove hazard from hand and deduct AP
            player.Hand.Remove(HazardCardUniqueId);
            player.AP -= ((Cards.IAPCostCard)hazardCard.Data).APCost;

            // Junk the hazard card after use
            player.JunkPile.Add(hazardCard);

            // Apply damage to target
            var targetPlayer = state.GetPlayer(TargetPlayerIndex);
            var targetStack = targetPlayer.GetVehicleStack(TargetVehicleUniqueId);
            var targetEquip = targetStack.FindEquipment(TargetEquipmentUniqueId);

            var hazardData = (Cards.HazardCardData)hazardCard.Data;
            _originalTargetSPP = targetEquip.Data.SPP;
            var resultSPP = HazardTargetRules.ApplyDamage(targetEquip.Data.SPP, hazardData.SPPDamage);

            _wasJunked = HazardTargetRules.ShouldJunk(resultSPP);
            if (_wasJunked)
            {
                targetStack.RemoveEquipment(TargetEquipmentUniqueId);
                targetPlayer.JunkPile.Add(targetEquip);
            }
            // Note: if not junked, the damage doesn't permanently reduce SPP
            // (Hazards either junk the target or do nothing lasting).
            // The damage check is: does applying damage drop any stat to 0?
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo handles this via CommandProcessor
        }
    }
}
