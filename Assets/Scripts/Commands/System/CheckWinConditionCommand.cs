using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Evaluate both win conditions:
    /// 1. First player to move 3 vehicles through all 4 realms wins.
    /// 2. If a player can't draw (deck empty at draw phase), their opponent wins.
    /// </summary>
    public class CheckWinConditionCommand : ICommand
    {
        private GameResult _previousResult;

        public string Validate(GameState state)
        {
            return null; // Always valid to check
        }

        public void Execute(GameState state)
        {
            _previousResult = state.Result;

            // Check vehicles finished win condition
            for (int i = 0; i < 2; i++)
            {
                if (state.GetPlayer(i).VehiclesFinished >= Constants.VehiclesToWin)
                {
                    state.Result = i == 0 ? GameResult.Player0Wins : GameResult.Player1Wins;
                    return;
                }
            }

            // Deck-out is checked during draw phase (handled by DrawPhase)
        }

        public void Undo(GameState state)
        {
            state.Result = _previousResult;
        }
    }
}
