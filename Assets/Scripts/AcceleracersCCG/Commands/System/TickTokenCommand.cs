using System.Collections.Generic;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Decrement all countdown tokens on a vehicle. Tokens that hit 0 are removed.
    /// </summary>
    public class TickTokenCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int VehicleUniqueId { get; }
        public string TokenKey { get; }

        private int _previousValue;

        public TickTokenCommand(int playerIndex, int vehicleUniqueId, string tokenKey)
        {
            PlayerIndex = playerIndex;
            VehicleUniqueId = vehicleUniqueId;
            TokenKey = tokenKey;
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

            _previousValue = stack.Tokens.Get(TokenKey);
            stack.Tokens.Decrement(TokenKey);
        }

        public void Undo(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.GetVehicleStack(VehicleUniqueId);
            if (stack != null)
            {
                stack.Tokens.Set(TokenKey, _previousValue);
            }
        }
    }
}
