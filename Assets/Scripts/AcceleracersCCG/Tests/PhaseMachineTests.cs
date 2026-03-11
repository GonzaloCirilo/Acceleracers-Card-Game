using NUnit.Framework;
using System.Collections.Generic;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Tests
{
    [TestFixture]
    public class PhaseMachineTests
    {
        [Test]
        public void Setup_DrawsInitialHands()
        {
            var deck0 = TestHelpers.CreateTestDeck("p0");
            var deck1 = TestHelpers.CreateTestDeck("p1");
            var realms = new CardInstance[]
            {
                new CardInstance(TestHelpers.MakeRealm("r1", "Realm 1", 4, SPPCategory.Speed)),
                new CardInstance(TestHelpers.MakeRealm("r2", "Realm 2", 5, SPPCategory.Power)),
                new CardInstance(TestHelpers.MakeRealm("r3", "Realm 3", 6, SPPCategory.Performance)),
                new CardInstance(TestHelpers.MakeRealm("r4", "Realm 4", 7, SPPCategory.Speed)),
            };

            var controller = new GameController(42);
            controller.StartGame(deck0, deck1, realms);

            // Both players should have drawn 7 cards
            Assert.AreEqual(Constants.InitialHandSize, controller.State.Players[0].Hand.Count);
            Assert.AreEqual(Constants.InitialHandSize, controller.State.Players[1].Hand.Count);

            // First realm should be revealed
            Assert.IsTrue(controller.State.RealmTrack.IsRevealed(0));
        }

        [Test]
        public void DrawPhase_AutoDrawsOneCard()
        {
            var controller = SetupRunningGame();
            int handBefore = controller.State.ActivePlayer.Hand.Count;

            // Should be at Draw phase after setup, auto-processes to PlayVehicle
            // The draw phase is automatic, so after setup -> draw -> advance -> PlayVehicle
            Assert.AreEqual(GamePhaseId.PlayVehicle, controller.State.CurrentPhase);

            // Active player should have drawn 1 more card (7 initial + 1 draw = 8)
            Assert.AreEqual(Constants.InitialHandSize + 1, controller.State.ActivePlayer.Hand.Count);
        }

        [Test]
        public void PlayVehiclePhase_CanPlayOrPass()
        {
            var controller = SetupRunningGame();
            var legalCommands = controller.GetLegalCommands();

            // Should have at least EndPhaseCommand (pass)
            Assert.IsTrue(legalCommands.Count >= 1);

            // Pass
            var endPhase = new EndPhaseCommand(controller.State.ActivePlayerIndex);
            controller.SubmitCommand(endPhase);

            // Should now be in Action phase (after TuneUp auto-processes)
            Assert.AreEqual(GamePhaseId.Action, controller.State.CurrentPhase);
        }

        [Test]
        public void ActionPhase_EndPhase_GoesToDiscard()
        {
            var controller = SetupRunningGame();

            // Skip play vehicle
            controller.SubmitCommand(new EndPhaseCommand(controller.State.ActivePlayerIndex));
            Assert.AreEqual(GamePhaseId.Action, controller.State.CurrentPhase);

            // End action phase
            controller.SubmitCommand(new EndPhaseCommand(controller.State.ActivePlayerIndex));

            // Should be at Discard or EndTurn (if not over max hand)
            // Since initial 7 + 1 draw = 8 cards > 7, should be at Discard
            Assert.AreEqual(GamePhaseId.Discard, controller.State.CurrentPhase);
        }

        [Test]
        public void DiscardPhase_DiscardToSeven_AdvancesToEndTurn()
        {
            var controller = SetupRunningGame();
            var activeIdx = controller.State.ActivePlayerIndex;

            // Skip to discard
            controller.SubmitCommand(new EndPhaseCommand(activeIdx));
            controller.SubmitCommand(new EndPhaseCommand(activeIdx));

            // Should have 8 cards, need to discard 1
            Assert.AreEqual(GamePhaseId.Discard, controller.State.CurrentPhase);
            Assert.AreEqual(8, controller.State.ActivePlayer.Hand.Count);

            // Discard one card
            var cardToDiscard = controller.State.ActivePlayer.Hand.Cards[0];
            controller.SubmitCommand(new DiscardCardCommand(activeIdx, cardToDiscard.UniqueId));

            // Should auto-advance through EndTurn to next player's Draw->...->PlayVehicle
            Assert.AreEqual(7, controller.State.Players[activeIdx].Hand.Count);
        }

        [Test]
        public void FullTurnCycle_SwapsActivePlayer()
        {
            var controller = SetupRunningGame();
            int firstPlayer = controller.State.ActivePlayerIndex;

            // Complete a full turn
            CompleteTurn(controller);

            // Active player should have swapped
            Assert.AreNotEqual(firstPlayer, controller.State.ActivePlayerIndex);
        }

        private GameController SetupRunningGame()
        {
            var deck0 = TestHelpers.CreateTestDeck("p0");
            var deck1 = TestHelpers.CreateTestDeck("p1");
            var realms = new CardInstance[]
            {
                new CardInstance(TestHelpers.MakeRealm("r1", "Realm 1", 4, SPPCategory.Speed)),
                new CardInstance(TestHelpers.MakeRealm("r2", "Realm 2", 5, SPPCategory.Power)),
                new CardInstance(TestHelpers.MakeRealm("r3", "Realm 3", 6, SPPCategory.Performance)),
                new CardInstance(TestHelpers.MakeRealm("r4", "Realm 4", 7, SPPCategory.Speed)),
            };

            var controller = new GameController(42);
            controller.StartGame(deck0, deck1, realms);
            return controller;
        }

        private void CompleteTurn(GameController controller)
        {
            var idx = controller.State.ActivePlayerIndex;

            // Play Vehicle phase: pass
            if (controller.State.CurrentPhase == GamePhaseId.PlayVehicle)
                controller.SubmitCommand(new EndPhaseCommand(idx));

            // Action phase: end
            if (controller.State.CurrentPhase == GamePhaseId.Action)
                controller.SubmitCommand(new EndPhaseCommand(idx));

            // Discard if needed
            while (controller.State.CurrentPhase == GamePhaseId.Discard
                   && controller.State.ActivePlayer.Hand.IsOverMaxSize)
            {
                var card = controller.State.ActivePlayer.Hand.Cards[0];
                controller.SubmitCommand(new DiscardCardCommand(idx, card.UniqueId));
            }

            // End discard if at limit
            if (controller.State.CurrentPhase == GamePhaseId.Discard)
                controller.SubmitCommand(new EndPhaseCommand(idx));
        }
    }
}
