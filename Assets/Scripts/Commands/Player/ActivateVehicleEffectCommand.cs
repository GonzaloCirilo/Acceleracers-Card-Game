using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;
using AcceleracersCCG.Effects;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Commands.Player
{
    /// <summary>
    /// Activates an in-play vehicle's special effect ability.
    /// Sets a PendingChoice if the effect requires player input.
    /// If no valid targets exist, the effect is lost with no pending choice set.
    /// </summary>
    public class ActivateVehicleEffectCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int VehicleUniqueId { get; }

        public ActivateVehicleEffectCommand(int playerIndex, int vehicleUniqueId)
        {
            PlayerIndex = playerIndex;
            VehicleUniqueId = vehicleUniqueId;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.VehiclesInPlay.FirstOrDefault(s => s.Vehicle.UniqueId == VehicleUniqueId);
            if (stack == null) return "Vehicle not in play.";
            if (!stack.Vehicle.Data.HasEffect(EffectIds.TransferMod) &&
                !stack.Vehicle.Data.HasEffect(EffectIds.TransferModIgnoreModability))
                return "Vehicle has no activatable effect.";
            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.VehiclesInPlay.First(s => s.Vehicle.UniqueId == VehicleUniqueId);

            if (stack.Vehicle.Data.HasEffect(EffectIds.TransferMod))
                ExecuteTransferMod(state, player, stack, ignoreModability: false);
            else if (stack.Vehicle.Data.HasEffect(EffectIds.TransferModIgnoreModability))
                ExecuteTransferMod(state, player, stack, ignoreModability: true);
        }

        private void ExecuteTransferMod(GameState state, PlayerState player, Components.VehicleStack sourceStack, bool ignoreModability)
        {
            // Collect mods that have at least one valid target
            var eligibleMods = new List<int>();
            foreach (var mod in sourceStack.EquippedMods)
            {
                bool hasTarget = player.VehiclesInPlay.Any(t =>
                    t != sourceStack &&
                    ((VehicleCardData)t.Vehicle.Data).Team == Team.MetalManiacs &&
                    t.RealmIndex == sourceStack.RealmIndex &&
                    (ignoreModability || ModabilityRules.CanEquipMod(mod.Data, t.Vehicle.Data))
                );
                if (hasTarget) eligibleMods.Add(mod.UniqueId);
            }

            // No valid targets — effect is lost
            if (eligibleMods.Count == 0) return;

            state.PendingChoice = new PendingChoice(
                ChoiceType.TransferModSelectMod,
                PlayerIndex,
                sourceCardUniqueId: VehicleUniqueId,
                options: eligibleMods,
                ignoreModability: ignoreModability
            );
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}
