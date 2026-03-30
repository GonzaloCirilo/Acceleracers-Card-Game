using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects.Implementations
{
    /// <summary>
    /// When the vehicle is brought into play, the owner may return any number of Mods
    /// from their junk pile to their hand, paying 1 AP per Mod.
    /// If the junk pile has no Mods or the player has no AP, the effect is lost.
    /// </summary>
    public class RecoverModsForAPEffect : ICardEffect
    {
        public List<ICommand> Resolve(GameState state, CardEffectContext context)
        {
            if (context.Trigger != EffectTrigger.OnPlay)
                return new List<ICommand>();

            var player = state.GetPlayer(context.OwnerPlayerIndex);
            if (player.AP < 1) return new List<ICommand>();

            var modOptions = player.JunkPile.Cards
                .Where(c => c.Data.CardType == CardType.Mod)
                .Select(c => c.UniqueId)
                .ToList();

            if (modOptions.Count == 0) return new List<ICommand>();

            // Include pass (-1) so the player can stop at any time
            var options = new List<int>(modOptions) { PendingChoice.PassOptionId };

            var choice = new PendingChoice(
                ChoiceType.RecoverModsForAP,
                context.OwnerPlayerIndex,
                context.SourceCard.UniqueId,
                options);

            return new List<ICommand> { new SetPendingChoiceCommand(choice) };
        }
    }
}
