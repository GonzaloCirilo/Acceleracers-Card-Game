using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Move a vehicle to the next realm. System command triggered during Advance phase.
    /// </summary>
    public class AdvanceVehicleCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int VehicleUniqueId { get; }

        private int _previousRealmIndex;

        public AdvanceVehicleCommand(int playerIndex, int vehicleUniqueId)
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
            if (stack.HasFinished)
                return "Vehicle has already finished.";
            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.GetVehicleStack(VehicleUniqueId);

            _previousRealmIndex = stack.RealmIndex;
            stack.RealmIndex++;

            if (stack.HasFinished)
            {
                player.VehiclesFinished++;
            }
        }

        public void Undo(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.GetVehicleStack(VehicleUniqueId);
            if (stack != null)
            {
                if (stack.HasFinished || stack.RealmIndex > _previousRealmIndex)
                {
                    if (stack.RealmIndex >= Constants.RealmsPerRace && _previousRealmIndex < Constants.RealmsPerRace)
                        player.VehiclesFinished--;
                }
                stack.RealmIndex = _previousRealmIndex;
            }
        }
    }
}
