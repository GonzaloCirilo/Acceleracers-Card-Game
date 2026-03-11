using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.Player
{
    /// <summary>
    /// Player signals they are done with the current phase.
    /// </summary>
    public class EndPhaseCommand : ICommand
    {
        public int PlayerIndex { get; }

        public EndPhaseCommand(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public string Validate(GameState state)
        {
            if (state.ActivePlayerIndex != PlayerIndex)
                return "Not your turn.";
            return null;
        }

        public void Execute(GameState state)
        {
            // The phase machine handles the actual transition.
            // This command just signals readiness.
        }

        public void Undo(GameState state)
        {
            // No state change to undo
        }
    }
}
