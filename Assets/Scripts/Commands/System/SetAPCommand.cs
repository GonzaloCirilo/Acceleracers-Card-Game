using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Sets a player's AP to a specific value. Stores previous value for undo.
    /// </summary>
    public class SetAPCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int NewAP { get; }

        private int _previousAP;

        public SetAPCommand(int playerIndex, int newAP)
        {
            PlayerIndex = playerIndex;
            NewAP = newAP;
        }

        public string Validate(GameState state)
        {
            return null;
        }

        public void Execute(GameState state)
        {
            _previousAP = state.Players[PlayerIndex].AP;
            state.Players[PlayerIndex].AP = NewAP;
        }

        public void Undo(GameState state)
        {
            state.Players[PlayerIndex].AP = _previousAP;
        }
    }
}
