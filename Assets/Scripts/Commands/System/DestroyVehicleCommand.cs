using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Junk an entire vehicle stack (vehicle + all equipment) to junk pile.
    /// </summary>
    public class DestroyVehicleCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int VehicleUniqueId { get; }

        public DestroyVehicleCommand(int playerIndex, int vehicleUniqueId)
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

            // Junk all cards in the stack
            foreach (var card in stack.AllCards())
            {
                player.JunkPile.Add(card);
            }

            player.VehiclesInPlay.Remove(stack);
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}
