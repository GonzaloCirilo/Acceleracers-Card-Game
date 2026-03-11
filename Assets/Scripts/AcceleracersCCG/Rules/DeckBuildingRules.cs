using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Rules
{
    /// <summary>
    /// Validates deck construction rules.
    /// Max 80 cards, 1 of each Vehicle/AcceleCharger, max 3 of same Shift/Hazard/Mod.
    /// </summary>
    public static class DeckBuildingRules
    {
        public static List<string> Validate(IReadOnlyList<CardData> cards)
        {
            var errors = new List<string>();

            if (cards.Count > Constants.MaxDeckSize)
                errors.Add($"Deck has {cards.Count} cards, max is {Constants.MaxDeckSize}.");

            // Group by card ID to check duplicates
            var groups = cards.GroupBy(c => c.Id);
            foreach (var group in groups)
            {
                var card = group.First();
                int count = group.Count();

                switch (card.CardType)
                {
                    case CardType.Vehicle:
                    case CardType.AcceleCharger:
                        if (count > 1)
                            errors.Add($"Only 1 copy of {card.CardType} \"{card.Name}\" allowed, found {count}.");
                        break;

                    case CardType.Mod:
                    case CardType.Shift:
                    case CardType.Hazard:
                        if (count > Constants.MaxCopiesPerCard)
                            errors.Add($"Max {Constants.MaxCopiesPerCard} copies of \"{card.Name}\" allowed, found {count}.");
                        break;

                    case CardType.RacingRealm:
                        // Racing Realms are not part of the main deck
                        errors.Add($"Racing Realm \"{card.Name}\" should not be in the main deck.");
                        break;
                }
            }

            return errors;
        }
    }
}
