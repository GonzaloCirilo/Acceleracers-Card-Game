using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Commands.System;
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
        }

        public void OnExit(GameState state)
        {
        }

        public List<ICommand> GetAutoCommands(GameState state)
        {
            var commands = new List<ICommand>();

            // Increment turn number at start of draw phase
            commands.Add(new IncrementTurnCommand());

            var activePlayer = state.ActivePlayer;

            // Deck-out check
            if (activePlayer.Deck.IsEmpty)
            {
                // Opponent wins
                var result = state.ActivePlayerIndex == 0
                    ? GameResult.Player1Wins
                    : GameResult.Player0Wins;
                commands.Add(new SetGameResultCommand(result));
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
