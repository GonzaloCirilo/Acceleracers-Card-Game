using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;
using AcceleracersCCG.Effects;
using AcceleracersCCG.Effects.Implementations;
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

            var apError = ActionPointRules.ValidateCost(player, ((IAPCostCard)hazardCard.Data).APCost);
            if (apError != null) return apError;

            var targetPlayer = state.GetPlayer(TargetPlayerIndex);
            var targetStack = targetPlayer.GetVehicleStack(TargetVehicleUniqueId);
            if (targetStack == null)
                return "Target vehicle not in play.";

            var targetEquip = targetStack.FindEquipment(TargetEquipmentUniqueId);
            if (targetEquip == null)
                return "Target equipment not found on vehicle.";

            if (hazardCard.Data.HasEffect(EffectIds.ApplyCountdown) &&
                targetEquip.Data.CardType != CardType.Mod)
                return "This Hazard can only target Mods.";

            var targetError = HazardTargetRules.ValidateTarget(hazardCard.Data, targetEquip);
            if (targetError != null) return targetError;

            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var hazardCard = player.Hand.Get(HazardCardUniqueId);

            // Remove hazard from hand, deduct AP, junk the hazard
            player.Hand.Remove(HazardCardUniqueId);
            player.AP -= ((IAPCostCard)hazardCard.Data).APCost;
            player.JunkPile.Add(hazardCard);

            var targetPlayer = state.GetPlayer(TargetPlayerIndex);
            var targetStack = targetPlayer.GetVehicleStack(TargetVehicleUniqueId);
            var targetEquip = targetStack.FindEquipment(TargetEquipmentUniqueId);

            if (hazardCard.Data.HasEffect(EffectIds.ApplyCountdown))
            {
                // Place 4 countdown tokens on the target vehicle, keyed by the target equipment's ID.
                // TuneUpPhase ticks these each turn and junks the Mod when they reach 0.
                var tokenKey = $"{TimedDestructionEffect.TokenKey}_{targetEquip.UniqueId}";
                int turns = hazardCard.Data.GetEffectParam(EffectIds.ApplyCountdown, defaultValue: 4);
                targetStack.Tokens.Set(tokenKey, turns);
            }
            else
            {
                var hazardData = (HazardCardData)hazardCard.Data;
                var resultSPP = HazardTargetRules.ApplyDamage(((ISPPCard)targetEquip.Data).SPP, hazardData.SPPDamage);

                _wasJunked = HazardTargetRules.ShouldJunk(resultSPP);
                if (_wasJunked)
                {
                    targetStack.RemoveEquipment(TargetEquipmentUniqueId);
                    targetPlayer.JunkPile.Add(targetEquip);
                }
            }
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo handles this via CommandProcessor
        }
    }
}
