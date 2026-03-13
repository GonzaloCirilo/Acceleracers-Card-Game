using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Sets the active player index. Stores previous value for undo.
    /// </summary>
    public class SetActivePlayerCommand : ICommand
    {
        public int PlayerIndex { get; }

        private int _previousPlayerIndex;

        public SetActivePlayerCommand(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public string Validate(GameState state)
        {
            return null;
        }

        public void Execute(GameState state)
        {
            _previousPlayerIndex = state.ActivePlayerIndex;
            state.ActivePlayerIndex = PlayerIndex;
        }

        public void Undo(GameState state)
        {
            state.ActivePlayerIndex = _previousPlayerIndex;
        }
    }
}
