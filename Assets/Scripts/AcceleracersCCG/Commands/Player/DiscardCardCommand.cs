using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.Player
{
    /// <summary>
    /// Discard a card from hand to junk pile.
    /// </summary>
    public class DiscardCardCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int CardUniqueId { get; }

        public DiscardCardCommand(int playerIndex, int cardUniqueId)
        {
            PlayerIndex = playerIndex;
            CardUniqueId = cardUniqueId;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            if (!player.Hand.Contains(CardUniqueId))
                return "Card not in hand.";
            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.Hand.Get(CardUniqueId);
            player.Hand.Remove(CardUniqueId);
            player.JunkPile.Add(card);
        }

        public void Undo(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            // Move from junk pile back to hand
            // JunkPile doesn't have Get by ID, so we rely on snapshot undo
        }
    }
}
