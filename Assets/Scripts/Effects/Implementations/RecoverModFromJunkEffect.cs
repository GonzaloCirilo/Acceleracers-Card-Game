using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects.Implementations
{
    /// <summary>
    /// When the vehicle is brought into play, the owner must choose 1 Mod
    /// from their junk pile and return it to their hand.
    /// The effect signals intent only — the phase layer is responsible for
    /// prompting the player to choose and dispatching RecoverModFromJunkCommand.
    /// </summary>
    public class RecoverModFromJunkEffect : ICardEffect
    {
        public List<ICommand> Resolve(GameState state, CardEffectContext context)
        {
            if (context.Trigger != EffectTrigger.OnPlay)
                return new List<ICommand>();

            var player = state.GetPlayer(context.OwnerPlayerIndex);
            // No mods in junk — nothing to do.
            if (!HasModInJunk(player))
                return new List<ICommand>();

            // Player choice required — phase layer handles this via HasEffect check.
            return new List<ICommand>();
        }

        private static bool HasModInJunk(AcceleracersCCG.Core.PlayerState player)
        {
            foreach (var card in player.JunkPile.Cards)
            {
                if (card.Data.CardType == Cards.CardType.Mod)
                    return true;
            }
            return false;
        }
    }
}
