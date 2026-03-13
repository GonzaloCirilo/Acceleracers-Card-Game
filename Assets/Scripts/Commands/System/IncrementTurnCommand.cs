using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Increments the turn number. Undo decrements it.
    /// </summary>
    public class IncrementTurnCommand : ICommand
    {
        public string Validate(GameState state)
        {
            return null;
        }

        public void Execute(GameState state)
        {
            state.TurnNumber++;
        }

        public void Undo(GameState state)
        {
            state.TurnNumber--;
        }
    }
}
