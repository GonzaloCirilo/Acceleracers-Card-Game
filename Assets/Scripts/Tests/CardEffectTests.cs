using NUnit.Framework;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;
using AcceleracersCCG.Effects;
using AcceleracersCCG.Rules;
using AcceleracersCCG.StateMachine.Phases;

namespace AcceleracersCCG.Tests
{
    /// <summary>
    /// Tests for the Junk Realm (realm-scoped ignore-modability, retain-mods-on-advance)
    /// and Strato-Thruster (auto-advance-next-turn + self-junk) effects.
    /// </summary>
    [TestFixture]
    public class CardEffectTests
    {
        // ---- Junk Realm: ANY Mod on ANY Vehicle (modability waived at realm level) ----

        [Test]
        public void JunkRealm_IgnoreModability_AllowsNonMatchingMod()
        {
            var track = new RealmTrack();
            track.SetRealm(0, new CardInstance(TestHelpers.MakeRealm("junk", "Junk Realm",
                5, SPPCategory.Speed, TerrainIcon.Sand,
                effectIds: new[] { EffectIds.IgnoreModability })));
            track.Reveal(0);

            var mod = TestHelpers.MakeMod(modability: ModabilityIcon.OffRoad);
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(modability: ModabilityIcon.Street));
            var stack = new VehicleStack(vehicle); // RealmIndex 0

            Assert.IsNull(EquipRules.ValidateMod(mod, stack, track),
                "Junk Realm should waive modability matching.");
        }

        [Test]
        public void NormalRealm_ModabilityMismatch_Rejected()
        {
            var track = new RealmTrack();
            track.SetRealm(0, new CardInstance(TestHelpers.MakeRealm()));
            track.Reveal(0);

            var mod = TestHelpers.MakeMod(modability: ModabilityIcon.OffRoad);
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(modability: ModabilityIcon.Street));
            var stack = new VehicleStack(vehicle);

            Assert.IsNotNull(EquipRules.ValidateMod(mod, stack, track),
                "Without the realm effect, mismatched modability must be rejected.");
        }

        // ---- Strato-Thruster: auto-advance ignoring escape, then self-junk ----

        private static CardInstance MakeStrato() =>
            new CardInstance(TestHelpers.MakeMod("strato", "Strato-Thruster", 0, 0, 0, apCost: 5,
                effectIds: new[] { EffectIds.AutoAdvanceNextTurn }));

        [Test]
        public void StratoThruster_ForcesAdvance_AndJunksItself()
        {
            var state = TestHelpers.CreateTestGameState(); // realm 0 escapes at 4 Speed
            var processor = new CommandProcessor();

            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3, power: 3, perf: 3));
            var stack = new VehicleStack(vehicle);
            var strato = MakeStrato();
            stack.EquippedMods.Add(strato);
            state.Players[0].VehiclesInPlay.Add(stack);

            Assert.IsFalse(AdvanceRules.CanAdvance(stack, state.RealmTrack),
                "Vehicle should not be able to escape on its own.");

            RunAdvance(state, processor);

            Assert.AreEqual(1, stack.RealmIndex, "Should have force-advanced one realm.");
            Assert.AreEqual(0, stack.EquippedMods.Count, "Strato-Thruster should junk itself.");
            Assert.IsTrue(state.Players[0].JunkPile.Contains(strato.UniqueId));
        }

        [Test]
        public void StratoThruster_InJunkRealm_IsRetained_AndDoesNotReAdvance()
        {
            var state = TestHelpers.CreateTestGameState();
            // Realm 0 = Junk Realm (retains mods on advance), escape 4 Speed so the
            // vehicle can't escape on its own — only the mod advances it.
            state.RealmTrack.SetRealm(0, new CardInstance(TestHelpers.MakeRealm("junk", "Junk Realm",
                4, SPPCategory.Speed, TerrainIcon.Sand,
                effectIds: new[] { EffectIds.RetainModsOnAdvance })));
            state.RealmTrack.Reveal(0);

            var processor = new CommandProcessor();
            var vehicle = new CardInstance(TestHelpers.MakeVehicle(speed: 3, power: 3, perf: 3));
            var stack = new VehicleStack(vehicle);
            var strato = MakeStrato();
            stack.EquippedMods.Add(strato);
            state.Players[0].VehiclesInPlay.Add(stack);

            RunAdvance(state, processor);

            Assert.AreEqual(1, stack.RealmIndex, "Should have advanced once.");
            Assert.AreEqual(1, stack.EquippedMods.Count, "Junk Realm retains the mod.");
            Assert.IsTrue(stack.Tokens.Has($"{EffectIds.AutoAdvanceFiredTokenPrefix}_{strato.UniqueId}"),
                "Retained mod should be flagged as already fired.");

            // Next turn's advance: mod is retained but must not advance the vehicle again.
            // Realm 1 (Swamp, escape 5 Power) can't be escaped by a 3/3/3 vehicle.
            int realmBefore = stack.RealmIndex;
            RunAdvance(state, processor);
            Assert.AreEqual(realmBefore, stack.RealmIndex, "Retained mod must not re-advance.");
            Assert.AreEqual(1, stack.EquippedMods.Count, "Mod still retained.");
        }

        // ---- RetainsMods helper: realm-scoped and vehicle-scoped ----

        [Test]
        public void RetainsMods_TrueForRealmEffect_AndVehicleEffect()
        {
            var track = new RealmTrack();
            track.SetRealm(0, new CardInstance(TestHelpers.MakeRealm("junk", "Junk Realm",
                5, SPPCategory.Speed, effectIds: new[] { EffectIds.RetainModsOnAdvance })));
            track.SetRealm(1, new CardInstance(TestHelpers.MakeRealm("plain", "Plain Realm", 5, SPPCategory.Speed)));
            track.Reveal(0);
            track.Reveal(1);

            var plainVehicle = new VehicleStack(new CardInstance(TestHelpers.MakeVehicle()));
            Assert.IsTrue(AdvanceRules.RetainsMods(plainVehicle, track, 0), "Realm 0 retains mods.");
            Assert.IsFalse(AdvanceRules.RetainsMods(plainVehicle, track, 1), "Realm 1 does not.");

            var retainVehicle = new VehicleStack(new CardInstance(TestHelpers.MakeVehicle(
                effectIds: new[] { EffectIds.RetainModsOnAdvance })));
            Assert.IsTrue(AdvanceRules.RetainsMods(retainVehicle, track, 1),
                "Vehicle-level effect retains mods regardless of realm.");
        }

        private static void RunAdvance(GameState state, CommandProcessor processor)
        {
            var phase = new AdvancePhase();
            foreach (var cmd in phase.GetAutoCommands(state))
                processor.ExecuteUnchecked(cmd, state);
        }
    }
}
