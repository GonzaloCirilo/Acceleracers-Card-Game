using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Add or remove a terrain bonus token on a vehicle stack.
    /// The actual SPP bonus is calculated dynamically by SPPCalculator;
    /// this command tracks whether the bonus has been applied via tokens.
    /// </summary>
    public class ApplyTerrainBonusCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int VehicleUniqueId { get; }
        public bool Apply { get; } // true = add, false = remove

        private const string TerrainBonusToken = "terrain_bonus";

        public ApplyTerrainBonusCommand(int playerIndex, int vehicleUniqueId, bool apply)
        {
            PlayerIndex = playerIndex;
            VehicleUniqueId = vehicleUniqueId;
            Apply = apply;
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

            if (Apply)
                stack.Tokens.Set(TerrainBonusToken, 1);
            else
                stack.Tokens.Remove(TerrainBonusToken);
        }

        public void Undo(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stack = player.GetVehicleStack(VehicleUniqueId);
            if (stack != null)
            {
                if (Apply)
                    stack.Tokens.Remove(TerrainBonusToken);
                else
                    stack.Tokens.Set(TerrainBonusToken, 1);
            }
        }
    }
}
