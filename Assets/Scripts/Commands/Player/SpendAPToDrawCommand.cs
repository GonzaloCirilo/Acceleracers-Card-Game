using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Commands.Player
{
    /// <summary>
    /// Spend 1 AP to draw 1 card.
    /// </summary>
    public class SpendAPToDrawCommand : ICommand
    {
        public int PlayerIndex { get; }
        private int _drawnCardId;

        public SpendAPToDrawCommand(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);

            var apError = ActionPointRules.ValidateCost(player, Constants.APPerExtraDraw);
            if (apError != null) return apError;

            if (player.Deck.IsEmpty)
                return "Deck is empty.";

            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            player.AP -= Constants.APPerExtraDraw;
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
                player.AP += Constants.APPerExtraDraw;
            }
        }
    }
}
