using System.Collections.Generic;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;

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

            _strippedShifts = new List<CardInstance>(stack.EquippedShifts);
            _strippedAcceleCharger = stack.AcceleCharger;

            // Move shifts to junk
            foreach (var shift in _strippedShifts)
            {
                player.JunkPile.Add(shift);
            }
            stack.EquippedShifts.Clear();

            // Move AcceleCharger to junk
            if (stack.AcceleCharger != null)
            {
                player.JunkPile.Add(stack.AcceleCharger);
                stack.AcceleCharger = null;
            }
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}
