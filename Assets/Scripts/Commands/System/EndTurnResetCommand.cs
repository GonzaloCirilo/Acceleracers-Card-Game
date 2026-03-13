using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Resets per-turn flags (HasPlayedVehicleThisTurn, AP) for a player.
    /// Stores previous values for undo.
    /// </summary>
    public class EndTurnResetCommand : ICommand
    {
        public int PlayerIndex { get; }

        private bool _previousHasPlayedVehicle;
        private int _previousAP;

        public EndTurnResetCommand(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public string Validate(GameState state)
        {
            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.Players[PlayerIndex];
            _previousHasPlayedVehicle = player.HasPlayedVehicleThisTurn;
            _previousAP = player.AP;

            player.HasPlayedVehicleThisTurn = false;
            player.AP = 0;
        }

        public void Undo(GameState state)
        {
            var player = state.Players[PlayerIndex];
            player.HasPlayedVehicleThisTurn = _previousHasPlayedVehicle;
            player.AP = _previousAP;
        }
    }
}
