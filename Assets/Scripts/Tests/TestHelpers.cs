using System.Collections.Generic;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Tests
{
    /// <summary>
    /// Helper methods for creating test data.
    /// </summary>
    public static class TestHelpers
    {
        public static VehicleCardData MakeVehicle(string id = "v1", string name = "Test Vehicle",
            int speed = 3, int power = 3, int perf = 3,
            Team team = Team.TekuRacers,
            ModabilityIcon modability = ModabilityIcon.Street | ModabilityIcon.Race,
            TerrainIcon terrain = TerrainIcon.None)
        {
            return new VehicleCardData(id, name, new SPP(speed, power, perf),
                team: team, modabilityIcons: modability, terrainIcons: terrain);
        }

        public static ModCardData MakeMod(string id = "m1", string name = "Test Mod",
            int speed = 1, int power = 1, int perf = 1, int apCost = 1,
            ModabilityIcon modability = ModabilityIcon.Street,
            TerrainIcon terrain = TerrainIcon.None)
        {
            return new ModCardData(id, name, new SPP(speed, power, perf),
                apCost: apCost, modabilityIcons: modability, terrainIcons: terrain);
        }

        public static ShiftCardData MakeShift(string id = "s1", string name = "Test Shift",
            int speed = 2, int power = 0, int perf = 0, int apCost = 1,
            TerrainIcon terrain = TerrainIcon.None)
        {
            return new ShiftCardData(id, name, new SPP(speed, power, perf),
                apCost: apCost, terrainIcons: terrain);
        }

        public static AcceleChargerCardData MakeAcceleCharger(string id = "ac1", string name = "Test AcceleCharger",
            int speed = 1, int power = 1, int perf = 1, int apCost = 1,
            TerrainIcon terrain = TerrainIcon.None)
        {
            return new AcceleChargerCardData(id, name, new SPP(speed, power, perf),
                apCost: apCost, terrainIcons: terrain);
        }

        public static HazardCardData MakeHazard(string id = "h1", string name = "Test Hazard",
            int dmgSpeed = 2, int dmgPower = 0, int dmgPerf = 0, int apCost = 1,
            bool canTargetAC = false, bool canTargetVehicles = false)
        {
            return new HazardCardData(id, name, new SPP(dmgSpeed, dmgPower, dmgPerf),
                apCost: apCost,
                canTargetAcceleChargers: canTargetAC,
                canTargetVehicles: canTargetVehicles);
        }

        public static RacingRealmCardData MakeRealm(string id = "r1", string name = "Test Realm",
            int escapeValue = 5, SPPCategory escapeCategory = SPPCategory.Speed,
            TerrainIcon terrain = TerrainIcon.Sand)
        {
            return new RacingRealmCardData(id, name, escapeValue, escapeCategory, terrain);
        }

        /// <summary>
        /// Create a fully set up GameState with decks, realms, etc.
        /// </summary>
        public static GameState CreateTestGameState(int seed = 42)
        {
            var state = new GameState(seed);

            var realms = new CardInstance[]
            {
                new CardInstance(MakeRealm("r1", "Storm Realm", 4, SPPCategory.Speed, TerrainIcon.Rough)),
                new CardInstance(MakeRealm("r2", "Swamp Realm", 5, SPPCategory.Power, TerrainIcon.Mud)),
                new CardInstance(MakeRealm("r3", "Lava Realm", 6, SPPCategory.Performance, TerrainIcon.Rough)),
                new CardInstance(MakeRealm("r4", "Cosmic Realm", 7, SPPCategory.Speed, TerrainIcon.Slick)),
            };

            for (int i = 0; i < 4; i++)
            {
                state.RealmTrack.SetRealm(i, realms[i]);
                state.RealmTrack.Reveal(i);
            }

            return state;
        }

        /// <summary>
        /// Create a test deck with a mix of card types.
        /// </summary>
        public static List<CardInstance> CreateTestDeck(string prefix, int vehicleCount = 4,
            int modCount = 8, int shiftCount = 8, int hazardCount = 5, int acCount = 2)
        {
            var cards = new List<CardInstance>();

            for (int i = 0; i < vehicleCount; i++)
            {
                var team = i % 2 == 0 ? Team.TekuRacers : Team.MetalManiacs;
                cards.Add(new CardInstance(MakeVehicle($"{prefix}_v{i}", $"Vehicle {i}",
                    3 + i, 3 + i, 3 + i, team)));
            }

            for (int i = 0; i < modCount; i++)
            {
                cards.Add(new CardInstance(MakeMod($"{prefix}_m{i}", $"Mod {i}",
                    1, 1, 1, 1, ModabilityIcon.Street | ModabilityIcon.Race)));
            }

            for (int i = 0; i < shiftCount; i++)
            {
                cards.Add(new CardInstance(MakeShift($"{prefix}_s{i}", $"Shift {i}",
                    2, 0, 0, 1)));
            }

            for (int i = 0; i < hazardCount; i++)
            {
                cards.Add(new CardInstance(MakeHazard($"{prefix}_h{i}", $"Hazard {i}",
                    3, 0, 0, 1)));
            }

            for (int i = 0; i < acCount; i++)
            {
                cards.Add(new CardInstance(MakeAcceleCharger($"{prefix}_ac{i}", $"AcceleCharger {i}",
                    1, 1, 1, 1)));
            }

            return cards;
        }
    }
}
