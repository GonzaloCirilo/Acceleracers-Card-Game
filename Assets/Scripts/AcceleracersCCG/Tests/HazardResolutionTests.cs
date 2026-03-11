using NUnit.Framework;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Tests
{
    [TestFixture]
    public class HazardResolutionTests
    {
        [Test]
        public void HazardDamage_OneStatToZero_TargetJunked()
        {
            var targetSPP = new SPP(3, 5, 4);
            var damage = new SPP(3, 0, 0);

            var result = HazardTargetRules.ApplyDamage(targetSPP, damage);
            Assert.AreEqual(new SPP(0, 5, 4), result);
            Assert.IsTrue(HazardTargetRules.ShouldJunk(result));
        }

        [Test]
        public void HazardDamage_NoStatToZero_NotJunked()
        {
            var targetSPP = new SPP(5, 5, 5);
            var damage = new SPP(2, 2, 2);

            var result = HazardTargetRules.ApplyDamage(targetSPP, damage);
            Assert.AreEqual(new SPP(3, 3, 3), result);
            Assert.IsFalse(HazardTargetRules.ShouldJunk(result));
        }

        [Test]
        public void HazardDamage_MultipleWindowDamage()
        {
            var targetSPP = new SPP(3, 2, 4);
            var damage = new SPP(1, 3, 0); // Blank Performance window

            var result = HazardTargetRules.ApplyDamage(targetSPP, damage);
            Assert.AreEqual(new SPP(2, -1, 4), result);
            Assert.IsTrue(HazardTargetRules.ShouldJunk(result)); // Power went below 0
        }

        [Test]
        public void PlayHazard_JunksShift_FullFlow()
        {
            var state = TestHelpers.CreateTestGameState();
            var processor = new CommandProcessor();

            // Set up opponent vehicle with shift
            var oppVehicle = new CardInstance(TestHelpers.MakeVehicle("ov1"));
            var oppStack = new VehicleStack(oppVehicle);
            var shift = new CardInstance(TestHelpers.MakeShift(speed: 2, power: 3, perf: 2));
            oppStack.EquippedShifts.Add(shift);
            state.Players[1].VehiclesInPlay.Add(oppStack);

            // Player 0 plays hazard
            var hazard = new CardInstance(TestHelpers.MakeHazard(dmgSpeed: 5, dmgPower: 0, dmgPerf: 0, apCost: 1));
            state.Players[0].Hand.Add(hazard);
            state.Players[0].AP = 3;

            var cmd = new PlayHazardCommand(0, hazard.UniqueId, 1, oppVehicle.UniqueId, shift.UniqueId);
            processor.ExecuteUnchecked(cmd, state);

            // Shift speed was 2, hazard does 5 → speed = -3 → junked
            Assert.AreEqual(0, oppStack.EquippedShifts.Count);
            Assert.IsTrue(state.Players[1].JunkPile.Contains(shift.UniqueId));
        }

        [Test]
        public void PlayHazard_ModSurvives_IfNoStatHitsZero()
        {
            var state = TestHelpers.CreateTestGameState();
            var processor = new CommandProcessor();

            var oppVehicle = new CardInstance(TestHelpers.MakeVehicle("ov1"));
            var oppStack = new VehicleStack(oppVehicle);
            var mod = new CardInstance(TestHelpers.MakeMod(speed: 5, power: 5, perf: 5));
            oppStack.EquippedMods.Add(mod);
            state.Players[1].VehiclesInPlay.Add(oppStack);

            // Hazard does 2 speed, 0 power, 0 perf damage
            var hazard = new CardInstance(TestHelpers.MakeHazard(dmgSpeed: 2, dmgPower: 0, dmgPerf: 0, apCost: 1));
            state.Players[0].Hand.Add(hazard);
            state.Players[0].AP = 3;

            var cmd = new PlayHazardCommand(0, hazard.UniqueId, 1, oppVehicle.UniqueId, mod.UniqueId);
            processor.ExecuteUnchecked(cmd, state);

            // Mod had 5 speed, hazard does 2 → speed = 3. No stat at 0. Not junked.
            Assert.AreEqual(1, oppStack.EquippedMods.Count);
        }

        [Test]
        public void AcceleCharger_ImmuneToNormalHazard()
        {
            var hazard = TestHelpers.MakeHazard(canTargetAC: false);
            var ac = new CardInstance(TestHelpers.MakeAcceleCharger());

            Assert.IsFalse(HazardTargetRules.CanTarget(hazard, ac));
            Assert.IsNotNull(HazardTargetRules.ValidateTarget(hazard, ac));
        }

        [Test]
        public void AcceleCharger_VulnerableToSpecialHazard()
        {
            var hazard = TestHelpers.MakeHazard(canTargetAC: true);
            var ac = new CardInstance(TestHelpers.MakeAcceleCharger());

            Assert.IsTrue(HazardTargetRules.CanTarget(hazard, ac));
            Assert.IsNull(HazardTargetRules.ValidateTarget(hazard, ac));
        }
    }
}
