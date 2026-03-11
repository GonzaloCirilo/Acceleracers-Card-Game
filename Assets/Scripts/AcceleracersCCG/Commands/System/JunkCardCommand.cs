using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Move a specific equipment card from a vehicle stack to junk pile.
    /// </summary>
    public class JunkCardCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int VehicleUniqueId { get; }
        public int CardUniqueId { get; }

        public JunkCardCommand(int playerIndex, int vehicleUniqueId, int cardUniqueId)
        {
            PlayerIndex = playerIndex;
            VehicleUniqueId = vehicleUniqueId;
            CardUniqueId = cardUniqueId;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.GetVehicleStack(VehicleUniqueId);
            if (stack == null)
                return "Vehicle not in play.";
            if (stack.FindEquipment(CardUniqueId) == null)
                return "Card not found on vehicle.";
            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.GetVehicleStack(VehicleUniqueId);
            var card = stack.RemoveEquipment(CardUniqueId);
            if (card != null)
            {
                player.JunkPile.Add(card);
            }
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}
