using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects.Implementations
{
    /// <summary>
    /// When the vehicle is brought into play, the owner chooses 1 Mod from
    /// their junk pile and returns it to their hand.
    /// If the junk pile has no Mods, the effect is lost.
    /// </summary>
    public class RecoverModFromJunkEffect : ICardEffect
    {
        public List<ICommand> Resolve(GameState state, CardEffectContext context)
        {
            if (context.Trigger != EffectTrigger.OnPlay)
                return new List<ICommand>();

            var player = state.GetPlayer(context.OwnerPlayerIndex);
            var options = player.JunkPile.Cards
                .Where(c => c.Data.CardType == CardType.Mod)
                .Select(c => c.UniqueId)
                .ToList();

            // No valid targets — effect is lost.
            if (options.Count == 0)
                return new List<ICommand>();

            var choice = new PendingChoice(
                ChoiceType.RecoverModFromJunk,
                context.OwnerPlayerIndex,
                context.SourceCard.UniqueId,
                options);

            return new List<ICommand> { new SetPendingChoiceCommand(choice) };
        }
    }
}
