using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Sets a pending player choice on the game state.
    /// Blocks phase advancement until resolved by ResolveChoiceCommand.
    /// </summary>
    public class SetPendingChoiceCommand : ICommand
    {
        private readonly PendingChoice _choice;

        public SetPendingChoiceCommand(PendingChoice choice)
        {
            _choice = choice;
        }

        public string Validate(GameState state) => null;

        public void Execute(GameState state)
        {
            state.PendingChoice = _choice;
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}
