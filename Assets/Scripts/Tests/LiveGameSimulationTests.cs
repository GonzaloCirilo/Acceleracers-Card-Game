using System.Collections.Generic;
using NUnit.Framework;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;
using AcceleracersCCG.Simulation;

namespace AcceleracersCCG.Tests
{
    /// <summary>
    /// End-to-end smoke tests: build two decks + a 4-realm track, then drive a full
    /// game to completion with the naive AI. These validate that the whole live loop
    /// (setup -> draw -> advance -> play -> tune-up -> action -> discard -> end turn)
    /// runs without throwing and terminates.
    /// </summary>
    [TestFixture]
    public class LiveGameSimulationTests
    {
        private static GameController StartGame(int seed)
        {
            CardInstance.ResetIdCounter();

            var deck0 = TestHelpers.CreateTestDeck("p0");
            var deck1 = TestHelpers.CreateTestDeck("p1");

            var realms = new CardInstance[]
            {
                new CardInstance(TestHelpers.MakeRealm("r1", "Storm Realm", 4, SPPCategory.Speed, TerrainIcon.Rough)),
                new CardInstance(TestHelpers.MakeRealm("r2", "Swamp Realm", 5, SPPCategory.Power, TerrainIcon.Mud)),
                new CardInstance(TestHelpers.MakeRealm("r3", "Lava Realm", 6, SPPCategory.Performance, TerrainIcon.Rough)),
                new CardInstance(TestHelpers.MakeRealm("r4", "Cosmic Realm", 7, SPPCategory.Speed, TerrainIcon.Slick)),
            };

            var controller = new GameController(seed);
            controller.StartGame(deck0, deck1, realms);
            return controller;
        }

        [Test]
        public void FullGame_RunsToCompletion_WithoutThrowing()
        {
            var controller = StartGame(seed: 42);

            var result = GameSimulator.Run(controller, new GameSimulator.Options { Seed = 7 });

            Assert.IsFalse(result.HitTurnCap, "Game hit the turn cap instead of ending naturally.");
            Assert.AreNotEqual(GameResult.InProgress, result.Outcome, "Game did not reach a terminal result.");
            Assert.Greater(result.Turns, 0, "No turns elapsed.");
        }

        [Test]
        public void FullGame_IsDeterministic_ForFixedSeeds()
        {
            var a = GameSimulator.Run(StartGame(1), new GameSimulator.Options { Seed = 3 });
            var b = GameSimulator.Run(StartGame(1), new GameSimulator.Options { Seed = 3 });

            Assert.AreEqual(a.Outcome, b.Outcome);
            Assert.AreEqual(a.Turns, b.Turns);
            Assert.AreEqual(a.Commands, b.Commands);
        }

        [Test]
        public void FullGame_ProducesAWinner_AcrossManySeeds()
        {
            for (int seed = 0; seed < 10; seed++)
            {
                var result = GameSimulator.Run(StartGame(seed), new GameSimulator.Options { Seed = seed });
                Assert.AreNotEqual(GameResult.InProgress, result.Outcome,
                    $"Seed {seed} did not terminate.");
            }
        }
    }
}
