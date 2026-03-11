using NUnit.Framework;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Tests
{
    [TestFixture]
    public class RuleValidationTests
    {
        [Test]
        public void ModabilityRules_SharedIcon_CanEquip()
        {
            var mod = TestHelpers.MakeMod(modability: ModabilityIcon.Street);
            var vehicle = TestHelpers.MakeVehicle(modability: ModabilityIcon.Street | ModabilityIcon.Race);

            Assert.IsTrue(ModabilityRules.CanEquipMod(mod, vehicle));
            Assert.IsNull(ModabilityRules.ValidateModEquip(mod, vehicle));
        }

        [Test]
        public void ModabilityRules_NoSharedIcon_CannotEquip()
        {
            var mod = TestHelpers.MakeMod(modability: ModabilityIcon.OffRoad);
            var vehicle = TestHelpers.MakeVehicle(modability: ModabilityIcon.Street | ModabilityIcon.Race);

            Assert.IsFalse(ModabilityRules.CanEquipMod(mod, vehicle));
            Assert.IsNotNull(ModabilityRules.ValidateModEquip(mod, vehicle));
        }

        [Test]
        public void EquipRules_AcceleCharger_MaxOne()
        {
            var ac = TestHelpers.MakeAcceleCharger();
            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            var stack = new VehicleStack(vehicle);

            Assert.IsNull(EquipRules.ValidateAcceleCharger(ac, stack));

            // Equip one
            stack.AcceleCharger = new CardInstance(TestHelpers.MakeAcceleCharger(id: "ac2"));

            // Can't equip another
            Assert.IsNotNull(EquipRules.ValidateAcceleCharger(ac, stack));
        }

        [Test]
        public void ActionPointRules_BaseAP_Is3()
        {
            var player = new PlayerState(0);
            Assert.AreEqual(3, ActionPointRules.CalculateAP(player));
        }

        [Test]
        public void ActionPointRules_TeamBonusAP()
        {
            var player = new PlayerState(0);

            // Add 2 Teku vehicles
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v1", team: Team.TekuRacers))));
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v2", team: Team.TekuRacers))));

            Assert.AreEqual(4, ActionPointRules.CalculateAP(player)); // 3 + 1 team bonus
        }

        [Test]
        public void ActionPointRules_MultipleTeamBonuses()
        {
            var player = new PlayerState(0);

            // 2 Teku + 2 Metal Maniacs
            player.VehiclesInPlay.Add(new VehicleStack(new CardInstance(TestHelpers.MakeVehicle("v1", team: Team.TekuRacers))));
            player.VehiclesInPlay.Add(new VehicleStack(new CardInstance(TestHelpers.MakeVehicle("v2", team: Team.TekuRacers))));
            player.VehiclesInPlay.Add(new VehicleStack(new CardInstance(TestHelpers.MakeVehicle("v3", team: Team.MetalManiacs))));
            player.VehiclesInPlay.Add(new VehicleStack(new CardInstance(TestHelpers.MakeVehicle("v4", team: Team.MetalManiacs))));

            Assert.AreEqual(5, ActionPointRules.CalculateAP(player)); // 3 + 2
        }

        [Test]
        public void ActionPointRules_SingleVehiclePerTeam_NoBonus()
        {
            var player = new PlayerState(0);
            player.VehiclesInPlay.Add(new VehicleStack(new CardInstance(TestHelpers.MakeVehicle("v1", team: Team.TekuRacers))));
            player.VehiclesInPlay.Add(new VehicleStack(new CardInstance(TestHelpers.MakeVehicle("v2", team: Team.MetalManiacs))));

            Assert.AreEqual(3, ActionPointRules.CalculateAP(player)); // No bonus (need 2+ per team)
        }

        [Test]
        public void ActionPointRules_CanAfford()
        {
            var player = new PlayerState(0) { AP = 3 };
            Assert.IsTrue(ActionPointRules.CanAfford(player, 3));
            Assert.IsFalse(ActionPointRules.CanAfford(player, 4));
        }

        [Test]
        public void HazardTargetRules_CanTargetMod()
        {
            var hazard = TestHelpers.MakeHazard();
            var mod = new CardInstance(TestHelpers.MakeMod());
            Assert.IsTrue(HazardTargetRules.CanTarget(hazard, mod));
        }

        [Test]
        public void HazardTargetRules_CanTargetShift()
        {
            var hazard = TestHelpers.MakeHazard();
            var shift = new CardInstance(TestHelpers.MakeShift());
            Assert.IsTrue(HazardTargetRules.CanTarget(hazard, shift));
        }

        [Test]
        public void HazardTargetRules_AcceleChargerImmune_ByDefault()
        {
            var hazard = TestHelpers.MakeHazard(canTargetAC: false);
            var ac = new CardInstance(TestHelpers.MakeAcceleCharger());
            Assert.IsFalse(HazardTargetRules.CanTarget(hazard, ac));
        }

        [Test]
        public void HazardTargetRules_AcceleChargerTargetable_WhenAllowed()
        {
            var hazard = TestHelpers.MakeHazard(canTargetAC: true);
            var ac = new CardInstance(TestHelpers.MakeAcceleCharger());
            Assert.IsTrue(HazardTargetRules.CanTarget(hazard, ac));
        }

        [Test]
        public void HazardTargetRules_ApplyDamage_BlankWindowIgnored()
        {
            var targetSPP = new SPP(5, 3, 4);
            var damage = new SPP(2, 0, 1); // Power damage is 0 (blank window)

            var result = HazardTargetRules.ApplyDamage(targetSPP, damage);

            Assert.AreEqual(new SPP(3, 3, 3), result); // Power unchanged
        }

        [Test]
        public void HazardTargetRules_ShouldJunk_WhenStatHitsZero()
        {
            Assert.IsTrue(HazardTargetRules.ShouldJunk(new SPP(0, 3, 3)));
            Assert.IsTrue(HazardTargetRules.ShouldJunk(new SPP(3, 0, 3)));
            Assert.IsTrue(HazardTargetRules.ShouldJunk(new SPP(3, 3, 0)));
            Assert.IsFalse(HazardTargetRules.ShouldJunk(new SPP(1, 1, 1)));
        }

        [Test]
        public void AdvanceRules_MeetsEscape_CanAdvance()
        {
            var realmTrack = new RealmTrack();
            var realm = new CardInstance(TestHelpers.MakeRealm(escapeValue: 5, escapeCategory: SPPCategory.Speed));
            realmTrack.SetRealm(0, realm);
            realmTrack.Reveal(0);

            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 5));
            var stack = new VehicleStack(vehicle);

            Assert.IsTrue(AdvanceRules.CanAdvance(stack, realmTrack));
        }

        [Test]
        public void AdvanceRules_BelowEscape_CannotAdvance()
        {
            var realmTrack = new RealmTrack();
            var realm = new CardInstance(TestHelpers.MakeRealm(escapeValue: 5, escapeCategory: SPPCategory.Speed));
            realmTrack.SetRealm(0, realm);
            realmTrack.Reveal(0);

            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 4));
            var stack = new VehicleStack(vehicle);

            Assert.IsFalse(AdvanceRules.CanAdvance(stack, realmTrack));
        }

        [Test]
        public void PlayVehicleRules_Valid()
        {
            var state = TestHelpers.CreateTestGameState();
            state.CurrentPhase = GamePhaseId.PlayVehicle;
            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            state.Players[0].Hand.Add(vehicle);

            Assert.IsNull(PlayVehicleRules.Validate(state, 0, vehicle));
        }

        [Test]
        public void PlayVehicleRules_AlreadyPlayed_Invalid()
        {
            var state = TestHelpers.CreateTestGameState();
            state.CurrentPhase = GamePhaseId.PlayVehicle;
            state.Players[0].HasPlayedVehicleThisTurn = true;
            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            state.Players[0].Hand.Add(vehicle);

            Assert.IsNotNull(PlayVehicleRules.Validate(state, 0, vehicle));
        }

        [Test]
        public void DeckBuildingRules_ValidDeck()
        {
            var cards = new System.Collections.Generic.List<CardData>();
            cards.Add(TestHelpers.MakeVehicle("v1"));
            cards.Add(TestHelpers.MakeVehicle("v2"));
            cards.Add(TestHelpers.MakeMod("m1"));
            cards.Add(TestHelpers.MakeMod("m1"));
            cards.Add(TestHelpers.MakeMod("m1"));

            var errors = DeckBuildingRules.Validate(cards);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void DeckBuildingRules_DuplicateVehicle_Invalid()
        {
            var cards = new System.Collections.Generic.List<CardData>();
            cards.Add(TestHelpers.MakeVehicle("v1"));
            cards.Add(TestHelpers.MakeVehicle("v1"));

            var errors = DeckBuildingRules.Validate(cards);
            Assert.IsNotEmpty(errors);
        }

        [Test]
        public void DeckBuildingRules_TooManyCopies_Invalid()
        {
            var cards = new System.Collections.Generic.List<CardData>();
            for (int i = 0; i < 4; i++)
                cards.Add(TestHelpers.MakeMod("m1"));

            var errors = DeckBuildingRules.Validate(cards);
            Assert.IsNotEmpty(errors);
        }
    }
}
