using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects.Implementations
{
    /// <summary>
    /// Parameterized effect: recover 1 card of a specific type from junk pile.
    /// Effect ID format: "recover_card_from_junk:card_type"
    /// e.g. "recover_card_from_junk:hazard", "recover_card_from_junk:mod"
    /// </summary>
    public class RecoverCardFromJunkEffect : ICardEffect
    {
        private readonly CardType _cardType;

        public RecoverCardFromJunkEffect(CardType cardType)
        {
            _cardType = cardType;
        }

        public List<ICommand> Resolve(GameState state, CardEffectContext context)
        {
            if (context.Trigger != EffectTrigger.OnPlay)
                return new List<ICommand>();

            var player = state.GetPlayer(context.OwnerPlayerIndex);
            var options = player.JunkPile.Cards
                .Where(c => c.Data.CardType == _cardType)
                .Select(c => c.UniqueId)
                .ToList();

            if (options.Count == 0)
                return new List<ICommand>();

            var choice = new PendingChoice(
                ChoiceType.RecoverCardFromJunk,
                context.OwnerPlayerIndex,
                context.SourceCard.UniqueId,
                options);

            return new List<ICommand> { new SetPendingChoiceCommand(choice) };
        }

        public static CardType ParseCardType(string param)
        {
            return param.ToLowerInvariant() switch
            {
                "mod" => CardType.Mod,
                "hazard" => CardType.Hazard,
                "shift" => CardType.Shift,
                "accelecharger" => CardType.AcceleCharger,
                "vehicle" => CardType.Vehicle,
                _ => throw new System.ArgumentException($"Unknown card type: '{param}'")
            };
        }
    }
}
