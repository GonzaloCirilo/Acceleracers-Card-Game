using NUnit.Framework;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Tests
{
    [TestFixture]
    public class TeamBonusTests
    {
        [Test]
        public void ZeroVehicles_BaseAP()
        {
            var player = new PlayerState(0);
            Assert.AreEqual(3, ActionPointRules.CalculateAP(player));
        }

        [Test]
        public void OneVehicle_NoBonus()
        {
            var player = new PlayerState(0);
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v1", team: Team.TekuRacers))));
            Assert.AreEqual(3, ActionPointRules.CalculateAP(player));
        }

        [Test]
        public void TwoSameTeam_OneBonusAP()
        {
            var player = new PlayerState(0);
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v1", team: Team.TekuRacers))));
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v2", team: Team.TekuRacers))));
            Assert.AreEqual(4, ActionPointRules.CalculateAP(player));
        }

        [Test]
        public void ThreeSameTeam_StillOneBonusAP()
        {
            var player = new PlayerState(0);
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v1", team: Team.MetalManiacs))));
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v2", team: Team.MetalManiacs))));
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v3", team: Team.MetalManiacs))));
            Assert.AreEqual(4, ActionPointRules.CalculateAP(player));
        }

        [Test]
        public void TwoTeamsWithTwoPlus_TwoBonusAP()
        {
            var player = new PlayerState(0);
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v1", team: Team.TekuRacers))));
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v2", team: Team.TekuRacers))));
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v3", team: Team.MetalManiacs))));
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v4", team: Team.MetalManiacs))));
            Assert.AreEqual(5, ActionPointRules.CalculateAP(player));
        }

        [Test]
        public void MixedTeams_OnlyTeamsWithTwoPlusGetBonus()
        {
            var player = new PlayerState(0);
            // 2 Teku, 1 Metal Maniacs, 1 Silencerz
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v1", team: Team.TekuRacers))));
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v2", team: Team.TekuRacers))));
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v3", team: Team.MetalManiacs))));
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v4", team: Team.Silencerz))));
            Assert.AreEqual(4, ActionPointRules.CalculateAP(player)); // Only Teku bonus
        }

        [Test]
        public void TeamNone_NotCounted()
        {
            var player = new PlayerState(0);
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v1", team: Team.None))));
            player.VehiclesInPlay.Add(new VehicleStack(
                new CardInstance(TestHelpers.MakeVehicle("v2", team: Team.None))));
            Assert.AreEqual(3, ActionPointRules.CalculateAP(player)); // Team.None doesn't count
        }
    }
}
