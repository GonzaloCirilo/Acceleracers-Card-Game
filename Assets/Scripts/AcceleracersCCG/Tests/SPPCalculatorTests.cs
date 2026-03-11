using NUnit.Framework;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Tests
{
    [TestFixture]
    public class SPPCalculatorTests
    {
        private RealmTrack _realmTrack;

        [SetUp]
        public void SetUp()
        {
            _realmTrack = new RealmTrack();
            var realm = new CardInstance(TestHelpers.MakeRealm("r1", "Sand Realm", 5, SPPCategory.Speed, TerrainIcon.Sand));
            _realmTrack.SetRealm(0, realm);
            _realmTrack.Reveal(0);
        }

        [Test]
        public void Calculate_VehicleOnly_ReturnsBaseStats()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3, power: 4, perf: 5));
            var stack = new VehicleStack(vehicle);

            var result = SPPCalculator.Calculate(stack, _realmTrack);

            Assert.AreEqual(new SPP(3, 4, 5), result);
        }

        [Test]
        public void Calculate_WithMod_AddsModStats()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3, power: 3, perf: 3));
            var stack = new VehicleStack(vehicle);
            stack.EquippedMods.Add(new CardInstance(TestHelpers.MakeMod(speed: 1, power: 2, perf: 0)));

            var result = SPPCalculator.Calculate(stack, _realmTrack);

            Assert.AreEqual(new SPP(4, 5, 3), result);
        }

        [Test]
        public void Calculate_WithShift_AddsShiftStats()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3, power: 3, perf: 3));
            var stack = new VehicleStack(vehicle);
            stack.EquippedShifts.Add(new CardInstance(TestHelpers.MakeShift(speed: 2, power: 0, perf: 0)));

            var result = SPPCalculator.Calculate(stack, _realmTrack);

            Assert.AreEqual(new SPP(5, 3, 3), result);
        }

        [Test]
        public void Calculate_WithAcceleCharger_AddsStats()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3, power: 3, perf: 3));
            var stack = new VehicleStack(vehicle);
            stack.AcceleCharger = new CardInstance(TestHelpers.MakeAcceleCharger(speed: 1, power: 1, perf: 1));

            var result = SPPCalculator.Calculate(stack, _realmTrack);

            Assert.AreEqual(new SPP(4, 4, 4), result);
        }

        [Test]
        public void Calculate_WithTerrainMatch_AddsOneToAll()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3, power: 3, perf: 3,
                terrain: TerrainIcon.Sand));
            var stack = new VehicleStack(vehicle);
            stack.RealmIndex = 0;

            var result = SPPCalculator.Calculate(stack, _realmTrack);

            Assert.AreEqual(new SPP(4, 4, 4), result);
        }

        [Test]
        public void Calculate_NoTerrainMatch_NoBonus()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3, power: 3, perf: 3,
                terrain: TerrainIcon.Water));
            var stack = new VehicleStack(vehicle);
            stack.RealmIndex = 0;

            var result = SPPCalculator.Calculate(stack, _realmTrack);

            Assert.AreEqual(new SPP(3, 3, 3), result);
        }

        [Test]
        public void Calculate_ModTerrainMatchesRealm_GivesBonus()
        {
            // Vehicle has no terrain match, but mod does
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3, power: 3, perf: 3,
                terrain: TerrainIcon.None));
            var stack = new VehicleStack(vehicle);
            stack.EquippedMods.Add(new CardInstance(TestHelpers.MakeMod(speed: 1, power: 1, perf: 1,
                terrain: TerrainIcon.Sand)));

            var result = SPPCalculator.Calculate(stack, _realmTrack);

            // Base 3+1(mod)+1(terrain) = 5 for each
            Assert.AreEqual(new SPP(5, 5, 5), result);
        }

        [Test]
        public void Calculate_MultipleTerrainMatches_OnlyOneBonusApplied()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3, power: 3, perf: 3,
                terrain: TerrainIcon.Sand));
            var stack = new VehicleStack(vehicle);
            stack.EquippedMods.Add(new CardInstance(TestHelpers.MakeMod(speed: 1, power: 0, perf: 0,
                terrain: TerrainIcon.Sand)));

            var result = SPPCalculator.Calculate(stack, _realmTrack);

            // Base 3 + Mod 1 + Terrain 1 = 5 speed; Base 3 + Mod 0 + Terrain 1 = 4 power/perf
            Assert.AreEqual(new SPP(5, 4, 4), result);
        }

        [Test]
        public void Calculate_FullStack_AllEquipmentStacks()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3, power: 3, perf: 3));
            var stack = new VehicleStack(vehicle);
            stack.EquippedMods.Add(new CardInstance(TestHelpers.MakeMod(speed: 1, power: 1, perf: 1)));
            stack.EquippedMods.Add(new CardInstance(TestHelpers.MakeMod(id: "m2", speed: 0, power: 2, perf: 0)));
            stack.EquippedShifts.Add(new CardInstance(TestHelpers.MakeShift(speed: 2, power: 0, perf: 0)));
            stack.AcceleCharger = new CardInstance(TestHelpers.MakeAcceleCharger(speed: 1, power: 1, perf: 1));

            var result = SPPCalculator.Calculate(stack, _realmTrack);

            // S: 3+1+0+2+1=7, P: 3+1+2+0+1=7, Perf: 3+1+0+0+1=5
            Assert.AreEqual(new SPP(7, 7, 5), result);
        }
    }
}
