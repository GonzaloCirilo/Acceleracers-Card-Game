using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Sets a token value on a vehicle stack. Removes the token if newValue is less than or equal to 0.
    /// </summary>
    public class SetTokenCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int VehicleUniqueId { get; }
        public string TokenKey { get; }
        public int NewValue { get; }

        private int _previousValue;
        private bool _hadToken;

        public SetTokenCommand(int playerIndex, int vehicleUniqueId, string tokenKey, int newValue)
        {
            PlayerIndex = playerIndex;
            VehicleUniqueId = vehicleUniqueId;
            TokenKey = tokenKey;
            NewValue = newValue;
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
            _hadToken = _previousValue > 0;

            if (NewValue <= 0)
            {
                stack.Tokens.Remove(TokenKey);
            }
            else
            {
                stack.Tokens.Set(TokenKey, NewValue);
            }
        }

        public void Undo(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.GetVehicleStack(VehicleUniqueId);
            if (stack == null) return;

            if (_hadToken)
            {
                stack.Tokens.Set(TokenKey, _previousValue);
            }
            else
            {
                stack.Tokens.Remove(TokenKey);
            }
        }
    }
}
