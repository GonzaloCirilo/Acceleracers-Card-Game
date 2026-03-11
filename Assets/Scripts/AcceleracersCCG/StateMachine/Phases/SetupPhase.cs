using System;
using System.Collections.Generic;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Core;
using AcceleracersCCG.Infrastructure;

namespace AcceleracersCCG.StateMachine.Phases
{
    /// <summary>
    /// Setup phase: coin flip, realm placement, shuffle decks, initial draw.
    /// Mulligan if no vehicle in hand.
    /// </summary>
    public class SetupPhase : IGamePhase
    {
        public GamePhaseId Id => GamePhaseId.Setup;
        public bool IsAutomatic => true;

        private readonly IRandomProvider _rng;

        public SetupPhase(IRandomProvider rng)
        {
            _rng = rng;
        }

        public void OnEnter(GameState state)
        {
        }

        public void OnExit(GameState state)
        {
        }

        public List<ICommand> GetAutoCommands(GameState state)
        {
            var commands = new List<ICommand>();

            // Coin flip to determine first player
            state.ActivePlayerIndex = _rng.Next(2);

            // Reveal first realm
            commands.Add(new FlipRealmCommand(0));

            // Shuffle both decks and draw initial hands
            var rng = new Random(_rng.Next(int.MaxValue));
            state.Players[0].Deck.Shuffle(rng);
            rng = new Random(_rng.Next(int.MaxValue));
            state.Players[1].Deck.Shuffle(rng);

            // Draw initial hands
            for (int i = 0; i < Constants.InitialHandSize; i++)
            {
                commands.Add(new DrawCardCommand(0));
                commands.Add(new DrawCardCommand(1));
            }

            return commands;
        }

        public List<ICommand> GetLegalPlayerCommands(GameState state)
        {
            return new List<ICommand>();
        }

        public GamePhaseId GetNextPhase(GameState state)
        {
            return GamePhaseId.Draw;
        }
    }
}
