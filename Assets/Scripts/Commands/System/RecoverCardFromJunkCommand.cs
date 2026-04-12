using System.Linq;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Move a specific card from a player's junk pile to their hand.
    /// </summary>
    public class RecoverCardFromJunkCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int CardUniqueId { get; }

        public RecoverCardFromJunkCommand(int playerIndex, int cardUniqueId)
        {
            PlayerIndex = playerIndex;
            CardUniqueId = cardUniqueId;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.JunkPile.Cards.FirstOrDefault(c => c.UniqueId == CardUniqueId);
            if (card == null) return "Card not in junk pile.";
            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.JunkPile.Cards.FirstOrDefault(c => c.UniqueId == CardUniqueId);
            player.JunkPile.Remove(CardUniqueId);
            player.Hand.Add(card);
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}
