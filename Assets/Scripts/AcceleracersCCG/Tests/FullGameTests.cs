using NUnit.Framework;
using System.Collections.Generic;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;
using AcceleracersCCG.Serialization;

namespace AcceleracersCCG.Tests
{
    [TestFixture]
    public class FullGameTests
    {
        private CardInstance[] _realms;
        private List<CardInstance> _deck0;
        private List<CardInstance> _deck1;

        [SetUp]
        public void SetUp()
        {
            _realms = new CardInstance[]
            {
                new CardInstance(TestHelpers.MakeRealm("r1", "Storm Realm", 4, SPPCategory.Speed, TerrainIcon.Rough)),
                new CardInstance(TestHelpers.MakeRealm("r2", "Swamp Realm", 5, SPPCategory.Power, TerrainIcon.Mud)),
                new CardInstance(TestHelpers.MakeRealm("r3", "Lava Realm", 5, SPPCategory.Performance, TerrainIcon.Rough)),
                new CardInstance(TestHelpers.MakeRealm("r4", "Cosmic Realm", 6, SPPCategory.Speed, TerrainIcon.Slick)),
            };

            _deck0 = TestHelpers.CreateTestDeck("p0");
            _deck1 = TestHelpers.CreateTestDeck("p1");
        }

        [Test]
        public void GameStartsCorrectly()
        {
            var controller = new GameController(42);
            controller.StartGame(_deck0, _deck1, _realms);

            Assert.AreEqual(GameResult.InProgress, controller.State.Result);
            Assert.AreEqual(Constants.InitialHandSize, controller.State.Players[0].Hand.Count);
            Assert.AreEqual(Constants.InitialHandSize, controller.State.Players[1].Hand.Count);
            Assert.IsTrue(controller.State.RealmTrack.IsRevealed(0));
        }

        [Test]
        public void CanPlayFullTurn()
        {
            var controller = new GameController(42);
            controller.StartGame(_deck0, _deck1, _realms);

            int activeIdx = controller.State.ActivePlayerIndex;

            // Should be at PlayVehicle phase (after auto draw + advance)
            Assert.AreEqual(GamePhaseId.PlayVehicle, controller.State.CurrentPhase);

            // Pass play vehicle
            controller.SubmitCommand(new EndPhaseCommand(activeIdx));
            Assert.AreEqual(GamePhaseId.Action, controller.State.CurrentPhase);

            // End action
            controller.SubmitCommand(new EndPhaseCommand(activeIdx));

            // Should be in Discard (8 cards > 7)
            if (controller.State.CurrentPhase == GamePhaseId.Discard)
            {
                while (controller.State.Players[activeIdx].Hand.IsOverMaxSize)
                {
                    var card = controller.State.Players[activeIdx].Hand.Cards[0];
                    controller.SubmitCommand(new DiscardCardCommand(activeIdx, card.UniqueId));
                }

                if (controller.State.CurrentPhase == GamePhaseId.Discard)
                    controller.SubmitCommand(new EndPhaseCommand(activeIdx));
            }

            // Should now be other player's turn at PlayVehicle
            Assert.AreNotEqual(activeIdx, controller.State.ActivePlayerIndex);
        }

        [Test]
        public void PlayVehicle_ThenEquipMod()
        {
            var controller = new GameController(42);
            controller.StartGame(_deck0, _deck1, _realms);

            int activeIdx = controller.State.ActivePlayerIndex;
            var player = controller.State.ActivePlayer;

            // Find a vehicle in hand
            var vehicles = player.Hand.GetByType(CardType.Vehicle);
            if (vehicles.Count > 0)
            {
                // Play vehicle
                var vehicleCard = vehicles[0];
                controller.SubmitCommand(new PlayVehicleCommand(activeIdx, vehicleCard.UniqueId));
                Assert.AreEqual(1, player.VehiclesInPlay.Count);

                // Move to action phase
                controller.SubmitCommand(new EndPhaseCommand(activeIdx));

                // Now in Action phase with AP
                Assert.AreEqual(GamePhaseId.Action, controller.State.CurrentPhase);
                Assert.IsTrue(player.AP >= Constants.BaseAP);

                // Find a mod in hand
                var mods = player.Hand.GetByType(CardType.Mod);
                if (mods.Count > 0)
                {
                    var mod = mods[0];
                    var vehicleStack = player.VehiclesInPlay[0];
                    var result = controller.SubmitCommand(
                        new EquipModCommand(activeIdx, mod.UniqueId, vehicleStack.Vehicle.UniqueId));

                    if (result == null)
                    {
                        Assert.AreEqual(1, vehicleStack.EquippedMods.Count);
                    }
                }
            }
        }

        [Test]
        public void DeterministicReplay_SameSeed_SameOutcome()
        {
            // Play a few turns with seed 42
            var controller1 = new GameController(42);
            controller1.StartGame(
                TestHelpers.CreateTestDeck("p0"),
                TestHelpers.CreateTestDeck("p1"),
                _realms);

            var hash1 = GameStateSerializer.ComputeHash(controller1.State);

            // Replay with same seed
            var controller2 = new GameController(42);
            controller2.StartGame(
                TestHelpers.CreateTestDeck("p0"),
                TestHelpers.CreateTestDeck("p1"),
                _realms);

            var hash2 = GameStateSerializer.ComputeHash(controller2.State);

            Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void GameController_ReportsGameOver()
        {
            var controller = new GameController(42);
            controller.StartGame(_deck0, _deck1, _realms);

            Assert.IsFalse(controller.IsGameOver);

            // Manually set win
            controller.State.Result = GameResult.Player0Wins;
            Assert.IsTrue(controller.IsGameOver);
        }

        [Test]
        public void SubmitCommand_AfterGameOver_ReturnsError()
        {
            var controller = new GameController(42);
            controller.StartGame(_deck0, _deck1, _realms);

            controller.State.Result = GameResult.Player0Wins;

            var error = controller.SubmitCommand(
                new EndPhaseCommand(controller.State.ActivePlayerIndex));
            Assert.IsNotNull(error);
            Assert.AreEqual("Game is over.", error);
        }
    }
}
