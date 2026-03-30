using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Commands.Player
{
    /// <summary>
    /// Moves a Mod from one vehicle stack to another Metal Maniac in the same Realm.
    /// Used by Rolling Thunder's effect. Free (no AP cost). Modability rules apply.
    /// </summary>
    public class TransferModCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int ModUniqueId { get; }
        public int TargetVehicleUniqueId { get; }
        public bool IgnoreModability { get; }

        public TransferModCommand(int playerIndex, int modUniqueId, int targetVehicleUniqueId, bool ignoreModability = false)
        {
            PlayerIndex = playerIndex;
            ModUniqueId = modUniqueId;
            TargetVehicleUniqueId = targetVehicleUniqueId;
            IgnoreModability = ignoreModability;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);

            var sourceStack = player.VehiclesInPlay.FirstOrDefault(s => s.EquippedMods.Any(m => m.UniqueId == ModUniqueId));
            if (sourceStack == null) return "Mod not found on any vehicle.";

            var targetStack = player.VehiclesInPlay.FirstOrDefault(s => s.Vehicle.UniqueId == TargetVehicleUniqueId);
            if (targetStack == null) return "Target vehicle not found.";
            if (targetStack == sourceStack) return "Target must be a different vehicle.";

            var targetVehicleData = (VehicleCardData)targetStack.Vehicle.Data;
            if (targetVehicleData.Team != Team.MetalManiacs) return "Target vehicle must be a Metal Maniac.";
            if (targetStack.RealmIndex != sourceStack.RealmIndex) return "Target vehicle must be in the same Realm.";

            if (IgnoreModability) return null;
            var mod = sourceStack.EquippedMods.First(m => m.UniqueId == ModUniqueId);
            return ModabilityRules.ValidateModEquip(mod.Data, targetStack.Vehicle.Data);
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var sourceStack = player.VehiclesInPlay.First(s => s.EquippedMods.Any(m => m.UniqueId == ModUniqueId));
            var targetStack = player.VehiclesInPlay.First(s => s.Vehicle.UniqueId == TargetVehicleUniqueId);

            var mod = sourceStack.RemoveEquipment(ModUniqueId);
            targetStack.EquippedMods.Add(mod);
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}
