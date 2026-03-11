using System;
using System.Collections.Generic;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Shuffles a player's deck using a given seed for replay determinism.
    /// Stores the pre-shuffle order for undo.
    /// </summary>
    public class ShuffleDeckCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int Seed { get; }

        private List<CardInstance> _previousOrder;

        public ShuffleDeckCommand(int playerIndex, int seed)
        {
            PlayerIndex = playerIndex;
            Seed = seed;
        }

        public string Validate(GameState state)
        {
            return null;
        }

        public void Execute(GameState state)
        {
            var deck = state.Players[PlayerIndex].Deck;
            _previousOrder = deck.ToList();
            deck.Shuffle(new Random(Seed));
        }

        public void Undo(GameState state)
        {
            var player = state.Players[PlayerIndex];
            player.Deck = new Components.Deck(_previousOrder);
        }
    }
}
