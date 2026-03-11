using NUnit.Framework;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Tests
{
    [TestFixture]
    public class CommandTests
    {
        private GameState _state;
        private CommandProcessor _processor;

        [SetUp]
        public void SetUp()
        {
            _state = TestHelpers.CreateTestGameState();
            _processor = new CommandProcessor();
        }

        [Test]
        public void DrawCardCommand_DrawsFromDeck()
        {
            var card = new CardInstance(TestHelpers.MakeVehicle());
            _state.Players[0].Deck.AddToTop(card);

            var cmd = new DrawCardCommand(0);
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.AreEqual(1, _state.Players[0].Hand.Count);
            Assert.AreEqual(0, _state.Players[0].Deck.Count);
        }

        [Test]
        public void DrawCardCommand_EmptyDeck_FailsValidation()
        {
            var cmd = new DrawCardCommand(0);
            Assert.IsNotNull(cmd.Validate(_state));
        }

        [Test]
        public void PlayVehicleCommand_PlacesVehicleAtRealm0()
        {
            _state.CurrentPhase = GamePhaseId.PlayVehicle;
            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            _state.Players[0].Hand.Add(vehicle);

            var cmd = new PlayVehicleCommand(0, vehicle.UniqueId);
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.AreEqual(0, _state.Players[0].Hand.Count);
            Assert.AreEqual(1, _state.Players[0].VehiclesInPlay.Count);
            Assert.AreEqual(0, _state.Players[0].VehiclesInPlay[0].RealmIndex);
            Assert.IsTrue(_state.Players[0].HasPlayedVehicleThisTurn);
        }

        [Test]
        public void EquipModCommand_EquipsAndDeductsAP()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            _state.Players[0].VehiclesInPlay.Add(new VehicleStack(vehicle));
            var mod = new CardInstance(TestHelpers.MakeMod(apCost: 2));
            _state.Players[0].Hand.Add(mod);
            _state.Players[0].AP = 5;

            var cmd = new EquipModCommand(0, mod.UniqueId, vehicle.UniqueId);
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.AreEqual(0, _state.Players[0].Hand.Count);
            Assert.AreEqual(1, _state.Players[0].VehiclesInPlay[0].EquippedMods.Count);
            Assert.AreEqual(3, _state.Players[0].AP);
        }

        [Test]
        public void EquipShiftCommand_EquipsAndDeductsAP()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            _state.Players[0].VehiclesInPlay.Add(new VehicleStack(vehicle));
            var shift = new CardInstance(TestHelpers.MakeShift(apCost: 1));
            _state.Players[0].Hand.Add(shift);
            _state.Players[0].AP = 3;

            var cmd = new EquipShiftCommand(0, shift.UniqueId, vehicle.UniqueId);
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.AreEqual(1, _state.Players[0].VehiclesInPlay[0].EquippedShifts.Count);
            Assert.AreEqual(2, _state.Players[0].AP);
        }

        [Test]
        public void EquipAcceleChargerCommand_EquipsMax1()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            _state.Players[0].VehiclesInPlay.Add(new VehicleStack(vehicle));
            var ac = new CardInstance(TestHelpers.MakeAcceleCharger(apCost: 1));
            _state.Players[0].Hand.Add(ac);
            _state.Players[0].AP = 3;

            var cmd = new EquipAcceleChargerCommand(0, ac.UniqueId, vehicle.UniqueId);
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.IsNotNull(_state.Players[0].VehiclesInPlay[0].AcceleCharger);

            // Try equipping another
            var ac2 = new CardInstance(TestHelpers.MakeAcceleCharger(id: "ac2", apCost: 1));
            _state.Players[0].Hand.Add(ac2);
            var cmd2 = new EquipAcceleChargerCommand(0, ac2.UniqueId, vehicle.UniqueId);
            Assert.IsNotNull(cmd2.Validate(_state));
        }

        [Test]
        public void PlayHazardCommand_DamagesAndJunksTarget()
        {
            // Set up opponent with vehicle + shift
            var oppVehicle = new CardInstance(TestHelpers.MakeVehicle("ov1"));
            var oppStack = new VehicleStack(oppVehicle);
            var shift = new CardInstance(TestHelpers.MakeShift(speed: 2, power: 1, perf: 1));
            oppStack.EquippedShifts.Add(shift);
            _state.Players[1].VehiclesInPlay.Add(oppStack);

            // Player 0 has a hazard that does 3 Speed damage
            var hazard = new CardInstance(TestHelpers.MakeHazard(dmgSpeed: 3, dmgPower: 0, dmgPerf: 0, apCost: 1));
            _state.Players[0].Hand.Add(hazard);
            _state.Players[0].AP = 3;

            var cmd = new PlayHazardCommand(0, hazard.UniqueId, 1, oppVehicle.UniqueId, shift.UniqueId);
            Assert.IsNull(cmd.Validate(_state));
            _processor.ExecuteUnchecked(cmd, _state);

            // Shift had Speed 2, hazard does 3 → Speed goes to -1 → junked
            Assert.AreEqual(0, oppStack.EquippedShifts.Count);
            Assert.IsTrue(_state.Players[1].JunkPile.Contains(shift.UniqueId));
            Assert.AreEqual(2, _state.Players[0].AP);
        }

        [Test]
        public void SpendAPToDrawCommand_DrawsAndDeductsAP()
        {
            var card = new CardInstance(TestHelpers.MakeMod());
            _state.Players[0].Deck.AddToTop(card);
            _state.Players[0].AP = 3;

            var cmd = new SpendAPToDrawCommand(0);
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.AreEqual(1, _state.Players[0].Hand.Count);
            Assert.AreEqual(2, _state.Players[0].AP);
        }

        [Test]
        public void DiscardCardCommand_MovesToJunkPile()
        {
            var card = new CardInstance(TestHelpers.MakeMod());
            _state.Players[0].Hand.Add(card);

            var cmd = new DiscardCardCommand(0, card.UniqueId);
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.AreEqual(0, _state.Players[0].Hand.Count);
            Assert.IsTrue(_state.Players[0].JunkPile.Contains(card.UniqueId));
        }

        [Test]
        public void AdvanceVehicleCommand_IncrementsRealmIndex()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            var stack = new VehicleStack(vehicle);
            _state.Players[0].VehiclesInPlay.Add(stack);

            var cmd = new AdvanceVehicleCommand(0, vehicle.UniqueId);
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.AreEqual(1, stack.RealmIndex);
        }

        [Test]
        public void AdvanceVehicleCommand_FourthRealm_IncrementsFinished()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            var stack = new VehicleStack(vehicle) { RealmIndex = 3 };
            _state.Players[0].VehiclesInPlay.Add(stack);

            var cmd = new AdvanceVehicleCommand(0, vehicle.UniqueId);
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.AreEqual(4, stack.RealmIndex);
            Assert.IsTrue(stack.HasFinished);
            Assert.AreEqual(1, _state.Players[0].VehiclesFinished);
        }

        [Test]
        public void StripTemporariesCommand_RemovesShiftsAndAC()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            var stack = new VehicleStack(vehicle);
            stack.EquippedShifts.Add(new CardInstance(TestHelpers.MakeShift()));
            stack.EquippedShifts.Add(new CardInstance(TestHelpers.MakeShift(id: "s2")));
            stack.AcceleCharger = new CardInstance(TestHelpers.MakeAcceleCharger());
            stack.EquippedMods.Add(new CardInstance(TestHelpers.MakeMod()));
            _state.Players[0].VehiclesInPlay.Add(stack);

            var cmd = new StripTemporariesCommand(0, vehicle.UniqueId);
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.AreEqual(0, stack.EquippedShifts.Count);
            Assert.IsNull(stack.AcceleCharger);
            Assert.AreEqual(1, stack.EquippedMods.Count); // Mods survive
            Assert.AreEqual(3, _state.Players[0].JunkPile.Count); // 2 shifts + 1 AC
        }

        [Test]
        public void DestroyVehicleCommand_JunksEntireStack()
        {
            var vehicle = new CardInstance(TestHelpers.MakeVehicle());
            var stack = new VehicleStack(vehicle);
            stack.EquippedMods.Add(new CardInstance(TestHelpers.MakeMod()));
            _state.Players[0].VehiclesInPlay.Add(stack);

            var cmd = new DestroyVehicleCommand(0, vehicle.UniqueId);
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.AreEqual(0, _state.Players[0].VehiclesInPlay.Count);
            Assert.AreEqual(2, _state.Players[0].JunkPile.Count); // Vehicle + Mod
        }

        [Test]
        public void CheckWinConditionCommand_VehiclesFinished_SetsResult()
        {
            _state.Players[0].VehiclesFinished = 3;

            var cmd = new CheckWinConditionCommand();
            _processor.ExecuteUnchecked(cmd, _state);

            Assert.AreEqual(GameResult.Player0Wins, _state.Result);
        }

        [Test]
        public void Undo_RestoresState()
        {
            var card = new CardInstance(TestHelpers.MakeVehicle());
            _state.Players[0].Deck.AddToTop(card);

            var cmd = new DrawCardCommand(0);
            _processor.Execute(cmd, _state);

            Assert.AreEqual(1, _state.Players[0].Hand.Count);
            Assert.AreEqual(0, _state.Players[0].Deck.Count);

            // Undo
            _processor.Undo(_state);

            Assert.AreEqual(0, _state.Players[0].Hand.Count);
            Assert.AreEqual(1, _state.Players[0].Deck.Count);
        }

        [Test]
        public void FlipRealmCommand_RevealsRealm()
        {
            Assert.IsTrue(_state.RealmTrack.IsRevealed(0)); // Already revealed in test setup

            // Un-reveal realm 1 for test (need fresh realm track)
            var state2 = new GameState();
            var realm = new CardInstance(TestHelpers.MakeRealm());
            state2.RealmTrack.SetRealm(1, realm);

            Assert.IsFalse(state2.RealmTrack.IsRevealed(1));

            var cmd = new FlipRealmCommand(1);
            _processor.ExecuteUnchecked(cmd, state2);

            Assert.IsTrue(state2.RealmTrack.IsRevealed(1));
        }
    }
}
