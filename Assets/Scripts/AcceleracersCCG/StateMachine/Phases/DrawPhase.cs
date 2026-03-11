using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.StateMachine.Phases
{
    /// <summary>
    /// Automatic: active player draws 1 card. If deck empty, opponent wins.
    /// </summary>
    public class DrawPhase : IGamePhase
    {
        public GamePhaseId Id => GamePhaseId.Draw;
        public bool IsAutomatic => true;

        public void OnEnter(GameState state)
        {
            // Increment turn number at start of draw phase
            state.TurnNumber++;
        }

        public void OnExit(GameState state)
        {
        }

        public List<ICommand> GetAutoCommands(GameState state)
        {
            var commands = new List<ICommand>();
            var activePlayer = state.ActivePlayer;

            // Deck-out check
            if (activePlayer.Deck.IsEmpty)
            {
                // Opponent wins
                state.Result = state.ActivePlayerIndex == 0
                    ? GameResult.Player1Wins
                    : GameResult.Player0Wins;
                return commands;
            }

            commands.Add(new DrawCardCommand(state.ActivePlayerIndex));
            return commands;
        }

        public List<ICommand> GetLegalPlayerCommands(GameState state)
        {
            return new List<ICommand>();
        }

        public GamePhaseId GetNextPhase(GameState state)
        {
            return GamePhaseId.Advance;
        }
    }
}
