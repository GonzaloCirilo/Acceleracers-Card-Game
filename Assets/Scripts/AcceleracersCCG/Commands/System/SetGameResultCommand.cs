using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Sets the game result (e.g., for deck-out wins). Stores previous value for undo.
    /// </summary>
    public class SetGameResultCommand : ICommand
    {
        public GameResult NewResult { get; }

        private GameResult _previousResult;

        public SetGameResultCommand(GameResult newResult)
        {
            NewResult = newResult;
        }

        public string Validate(GameState state)
        {
            return null;
        }

        public void Execute(GameState state)
        {
            _previousResult = state.Result;
            state.Result = NewResult;
        }

        public void Undo(GameState state)
        {
            state.Result = _previousResult;
        }
    }
}
