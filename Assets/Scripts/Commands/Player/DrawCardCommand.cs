using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.Player
{
    /// <summary>
    /// Draw a card from deck to hand.
    /// </summary>
    public class DrawCardCommand : ICommand
    {
        public int PlayerIndex { get; }

        // Set after execute for undo
        private int _drawnCardId;

        public DrawCardCommand(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            if (player.Deck.IsEmpty)
                return "Deck is empty.";
            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.Deck.Draw();
            _drawnCardId = card.UniqueId;
            player.Hand.Add(card);
        }

        public void Undo(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.Hand.Get(_drawnCardId);
            if (card != null)
            {
                player.Hand.Remove(_drawnCardId);
                player.Deck.AddToTop(card);
            }
        }
    }
}
