using NUnit.Framework;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Tests
{
    [TestFixture]
    public class AdvanceLogicTests
    {
        [Test]
        public void VehicleAdvances_WhenSPPMeetsEscape()
        {
            var realmTrack = new RealmTrack();
            realmTrack.SetRealm(0, new CardInstance(
                TestHelpers.MakeRealm("r1", "Realm 1", 5, SPPCategory.Speed)));
            realmTrack.Reveal(0);

            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 5));
            var stack = new VehicleStack(vehicle);

            Assert.IsTrue(AdvanceRules.CanAdvance(stack, realmTrack));
        }

        [Test]
        public void VehicleDoesNotAdvance_WhenSPPBelowEscape()
        {
            var realmTrack = new RealmTrack();
            realmTrack.SetRealm(0, new CardInstance(
                TestHelpers.MakeRealm("r1", "Realm 1", 5, SPPCategory.Speed)));
            realmTrack.Reveal(0);

            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 4));
            var stack = new VehicleStack(vehicle);

            Assert.IsFalse(AdvanceRules.CanAdvance(stack, realmTrack));
        }

        [Test]
        public void EquipmentHelpsVehicleAdvance()
        {
            var realmTrack = new RealmTrack();
            realmTrack.SetRealm(0, new CardInstance(
                TestHelpers.MakeRealm("r1", "Realm 1", 5, SPPCategory.Speed)));
            realmTrack.Reveal(0);

            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3));
            var stack = new VehicleStack(vehicle);
            stack.EquippedShifts.Add(new CardInstance(TestHelpers.MakeShift(speed: 2, power: 0, perf: 0)));

            Assert.IsTrue(AdvanceRules.CanAdvance(stack, realmTrack));
        }

        [Test]
        public void StripTemporaries_OnAdvance()
        {
            var state = TestHelpers.CreateTestGameState();
            var processor = new CommandProcessor();

            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            var stack = new VehicleStack(vehicle);
            stack.EquippedShifts.Add(new CardInstance(TestHelpers.MakeShift()));
            stack.AcceleCharger = new CardInstance(TestHelpers.MakeAcceleCharger());
            stack.EquippedMods.Add(new CardInstance(TestHelpers.MakeMod()));
            state.Players[0].VehiclesInPlay.Add(stack);

            // Strip temporaries
            processor.ExecuteUnchecked(new StripTemporariesCommand(0, vehicle.UniqueId), state);

            Assert.AreEqual(0, stack.EquippedShifts.Count);
            Assert.IsNull(stack.AcceleCharger);
            Assert.AreEqual(1, stack.EquippedMods.Count); // Mod persists
        }

        [Test]
        public void VehicleExitsFourthRealm_CountsAsFinished()
        {
            var state = TestHelpers.CreateTestGameState();
            var processor = new CommandProcessor();

            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 10, power: 10, perf: 10));
            var stack = new VehicleStack(vehicle) { RealmIndex = 3 };
            state.Players[0].VehiclesInPlay.Add(stack);

            processor.ExecuteUnchecked(new AdvanceVehicleCommand(0, vehicle.UniqueId), state);

            Assert.IsTrue(stack.HasFinished);
            Assert.AreEqual(4, stack.RealmIndex);
            Assert.AreEqual(1, state.Players[0].VehiclesFinished);
        }

        [Test]
        public void ThreeVehiclesFinished_WinsGame()
        {
            var state = TestHelpers.CreateTestGameState();
            var processor = new CommandProcessor();

            state.Players[0].VehiclesFinished = 3;

            processor.ExecuteUnchecked(new CheckWinConditionCommand(), state);

            Assert.AreEqual(GameResult.Player0Wins, state.Result);
        }

        [Test]
        public void VehicleAdvancesThroughAllRealms()
        {
            var state = TestHelpers.CreateTestGameState();
            var processor = new CommandProcessor();

            // Vehicle with high stats to pass all realms
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 10, power: 10, perf: 10));
            var stack = new VehicleStack(vehicle);
            state.Players[0].VehiclesInPlay.Add(stack);

            for (int i = 0; i < Constants.RealmsPerRace; i++)
            {
                Assert.AreEqual(i, stack.RealmIndex);
                Assert.IsTrue(AdvanceRules.CanAdvance(stack, state.RealmTrack));
                processor.ExecuteUnchecked(new AdvanceVehicleCommand(0, vehicle.UniqueId), state);
            }

            Assert.IsTrue(stack.HasFinished);
            Assert.AreEqual(1, state.Players[0].VehiclesFinished);
        }

        [Test]
        public void TerrainBonus_HelpsAdvance()
        {
            var realmTrack = new RealmTrack();
            realmTrack.SetRealm(0, new CardInstance(
                TestHelpers.MakeRealm("r1", "Sand Realm", 5, SPPCategory.Speed, TerrainIcon.Sand)));
            realmTrack.Reveal(0);

            // Vehicle has speed 4, needs 5. But has Sand terrain → +1 = 5
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 4, power: 4, perf: 4,
                terrain: TerrainIcon.Sand));
            var stack = new VehicleStack(vehicle);

            var totalSPP = SPPCalculator.Calculate(stack, realmTrack);
            Assert.AreEqual(5, totalSPP.Speed);
            Assert.IsTrue(AdvanceRules.CanAdvance(stack, realmTrack));
        }
    }
}
