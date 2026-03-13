using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;
using AcceleracersCCG.Effects;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Remove all Shifts and AcceleCharger from a vehicle stack (on realm advance).
    /// Stripped cards go to junk pile.
    /// </summary>
    public class StripTemporariesCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int VehicleUniqueId { get; }

        // For undo
        private List<CardInstance> _strippedShifts;
        private CardInstance _strippedAcceleCharger;

        public StripTemporariesCommand(int playerIndex, int vehicleUniqueId)
        {
            PlayerIndex = playerIndex;
            VehicleUniqueId = vehicleUniqueId;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.GetVehicleStack(VehicleUniqueId);
            if (stack == null)
                return "Vehicle not in play.";
            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.GetVehicleStack(VehicleUniqueId);

            _strippedShifts = stack.EquippedShifts
                .Where(s => s.Data.EffectId != EffectIds.PersistOnAdvance)
                .ToList();
            _strippedAcceleCharger = stack.AcceleCharger != null
                && stack.AcceleCharger.Data.EffectId != EffectIds.PersistOnAdvance
                ? stack.AcceleCharger
                : null;

            // Move non-persistent shifts to junk
            foreach (var shift in _strippedShifts)
            {
                stack.EquippedShifts.Remove(shift);
                player.JunkPile.Add(shift);
            }

            // Move AcceleCharger to junk (unless persistent)
            if (_strippedAcceleCharger != null)
            {
                player.JunkPile.Add(_strippedAcceleCharger);
                stack.AcceleCharger = null;
            }
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}
