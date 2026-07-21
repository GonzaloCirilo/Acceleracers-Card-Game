using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Cards.Data;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Simulation
{
    /// <summary>
    /// Builds a started <see cref="GameController"/> from the CardDataSO assets under
    /// Resources. Shared by the headless <see cref="GameBootstrap"/> and the visual
    /// <see cref="GameView"/> so both construct the game identically.
    /// </summary>
    public static class GameFactory
    {
        /// <summary>
        /// Loads cards from Resources/&lt;folder&gt;, builds two legal decks + a 4-realm
        /// track, and starts the game. Returns null (with a reason in <paramref name="summary"/>)
        /// if there aren't enough cards.
        /// </summary>
        public static GameController CreateFromResources(string cardResourceFolder, int gameSeed, out string summary)
        {
            var sos = Resources.LoadAll<CardDataSO>(cardResourceFolder);
            if (sos == null || sos.Length == 0)
            {
                summary = $"No CardDataSO assets found under Resources/{cardResourceFolder}.";
                return null;
            }

            var allCards = sos.Select(so => so.ToCardData()).ToList();
            var realmCards = allCards.Where(c => c.CardType == CardType.RacingRealm).ToList();
            var deckCards = allCards.Where(c => c.CardType != CardType.RacingRealm).ToList();

            if (realmCards.Count < Constants.RealmsPerRace)
            {
                summary = $"Need {Constants.RealmsPerRace} realms, found {realmCards.Count}.";
                return null;
            }
            if (deckCards.Count == 0)
            {
                summary = "No non-realm cards to build a deck from.";
                return null;
            }

            // One legal deck per player: a single copy of each distinct card (trivially
            // satisfies "1 vehicle/AcceleCharger, <=3 of others"), capped at MaxDeckSize.
            var template = deckCards.Take(Constants.MaxDeckSize).ToList();
            foreach (var e in DeckBuildingRules.Validate(template))
                Debug.LogWarning("[GameFactory] Deck rule: " + e);

            CardInstance.ResetIdCounter();
            var deck0 = template.Select(c => new CardInstance(c)).ToList();
            var deck1 = template.Select(c => new CardInstance(c)).ToList();
            var realms = realmCards.Take(Constants.RealmsPerRace)
                                   .Select(c => new CardInstance(c)).ToArray();

            var controller = new GameController(gameSeed);
            controller.StartGame(deck0, deck1, realms);

            summary = $"{allCards.Count} cards ({realmCards.Count} realms, {deckCards.Count} deck cards); " +
                      $"deck/player {template.Count}. First player P{controller.State.ActivePlayerIndex}.";
            return controller;
        }
    }
}
