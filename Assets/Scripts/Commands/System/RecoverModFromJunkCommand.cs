using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Move a specific Mod card from a player's junk pile to their hand.
    /// </summary>
    public class RecoverModFromJunkCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int ModUniqueId { get; }

        public RecoverModFromJunkCommand(int playerIndex, int modUniqueId)
        {
            PlayerIndex = playerIndex;
            ModUniqueId = modUniqueId;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.JunkPile.Cards.FirstOrDefault(c => c.UniqueId == ModUniqueId);
            if (card == null) return "Card not in junk pile.";
            if (card.Data.CardType != CardType.Mod) return "Card is not a Mod.";
            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.JunkPile.Cards.FirstOrDefault(c => c.UniqueId == ModUniqueId);
            player.JunkPile.Remove(ModUniqueId);
            player.Hand.Add(card);
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}
