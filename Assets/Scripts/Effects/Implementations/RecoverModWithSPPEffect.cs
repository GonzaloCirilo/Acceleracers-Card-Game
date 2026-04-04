using System;
using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects.Implementations
{
    /// <summary>
    /// When the vehicle is brought into play, the owner returns 1 Mod from their junk pile
    /// that has a non-blank value in a specific SPP category.
    /// Parameterized via effect ID: "recover_mod_with_spp:speed", "recover_mod_with_spp:power", etc.
    /// If no matching Mods exist in the junk pile, the effect is lost.
    /// </summary>
    public class RecoverModWithSPPEffect : ICardEffect
    {
        private readonly SPPCategory _category;

        public RecoverModWithSPPEffect(SPPCategory category)
        {
            _category = category;
        }

        public List<ICommand> Resolve(GameState state, CardEffectContext context)
        {
            if (context.Trigger != EffectTrigger.OnPlay)
                return new List<ICommand>();

            var player = state.GetPlayer(context.OwnerPlayerIndex);
            var options = player.JunkPile.Cards
                .Where(c => c.Data is ModCardData mod && mod.SPP.HasWindow(_category))
                .Select(c => c.UniqueId)
                .ToList();

            if (options.Count == 0)
                return new List<ICommand>();

            var choice = new PendingChoice(
                ChoiceType.RecoverModFromJunk,
                context.OwnerPlayerIndex,
                context.SourceCard.UniqueId,
                options);

            return new List<ICommand> { new SetPendingChoiceCommand(choice) };
        }

        public static SPPCategory ParseCategory(string param)
        {
            return param?.ToLowerInvariant() switch
            {
                "speed" => SPPCategory.Speed,
                "power" => SPPCategory.Power,
                "performance" => SPPCategory.Performance,
                _ => throw new ArgumentException($"Unknown SPP category: '{param}'")
            };
        }
    }
}
