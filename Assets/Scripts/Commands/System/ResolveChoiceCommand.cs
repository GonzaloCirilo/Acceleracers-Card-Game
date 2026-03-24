using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Resolves a pending player choice with a selected card unique ID.
    /// Clears PendingChoice and executes the appropriate action.
    /// </summary>
    public class ResolveChoiceCommand : ICommand
    {
        public int SelectedCardUniqueId { get; }

        public ResolveChoiceCommand(int selectedCardUniqueId)
        {
            SelectedCardUniqueId = selectedCardUniqueId;
        }

        public string Validate(GameState state)
        {
            if (state.PendingChoice == null)
                return "No pending choice to resolve.";
            if (!state.PendingChoice.Options.Contains(SelectedCardUniqueId))
                return "Selected card is not a valid option.";
            return null;
        }

        public void Execute(GameState state)
        {
            var choice = state.PendingChoice;
            state.PendingChoice = null;

            switch (choice.Type)
            {
                case ChoiceType.RecoverModFromJunk:
                    var recover = new RecoverModFromJunkCommand(choice.PlayerIndex, SelectedCardUniqueId);
                    recover.Execute(state);
                    break;
            }
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}
