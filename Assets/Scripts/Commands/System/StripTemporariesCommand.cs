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

            // Check if the vehicle retains equipment at this realm transition
            int advancingToRealm = stack.RealmIndex + 1; // 0-based
            bool vehicleRetainsShifts = false;
            bool vehicleRetainsAcceleCharger = false;
            foreach (var effectId in stack.Vehicle.Data.EffectIds)
            {
                int realm = EffectIds.ParseParam(effectId, EffectIds.RetainShiftsOnRealmAdvancePrefix);
                if (realm == advancingToRealm + 1) // param is 1-based, RealmIndex is 0-based
                    vehicleRetainsShifts = true;

                realm = EffectIds.ParseParam(effectId, EffectIds.RetainAcceleChargerOnRealmAdvancePrefix);
                if (realm == advancingToRealm + 1)
                    vehicleRetainsAcceleCharger = true;
            }

            _strippedShifts = stack.EquippedShifts
                .Where(s => !vehicleRetainsShifts && !s.Data.HasEffect(EffectIds.PersistOnAdvance))
                .ToList();
            _strippedAcceleCharger = stack.AcceleCharger != null
                && !vehicleRetainsAcceleCharger
                && !stack.AcceleCharger.Data.HasEffect(EffectIds.PersistOnAdvance)
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
