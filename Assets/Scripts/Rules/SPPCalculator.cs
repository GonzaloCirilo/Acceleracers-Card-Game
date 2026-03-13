using AcceleracersCCG.Components;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Rules
{
    /// <summary>
    /// Calculates the total SPP for a vehicle stack including all equipment and terrain bonuses.
    /// </summary>
    public static class SPPCalculator
    {
        /// <summary>
        /// Calculates the total SPP for a vehicle stack at a given realm.
        /// </summary>
        public static SPP Calculate(VehicleStack stack, RealmTrack realmTrack)
        {
            var total = stack.Vehicle.Data.SPP;

            // Add mods
            foreach (var mod in stack.EquippedMods)
            {
                total = total + mod.Data.SPP;
            }

            // Add shifts
            foreach (var shift in stack.EquippedShifts)
            {
                total = total + shift.Data.SPP;
            }

            // Add AcceleCharger
            if (stack.AcceleCharger != null)
            {
                total = total + stack.AcceleCharger.Data.SPP;
            }

            // Terrain bonus: +1 to ALL SPP if any card in the stack has a matching terrain icon
            var terrainBonus = CalculateTerrainBonus(stack, realmTrack);
            total = total + terrainBonus;

            return total;
        }

        /// <summary>
        /// Calculates the terrain bonus SPP. Returns SPP(1,1,1) if any card in the
        /// stack has a terrain icon matching the current realm, otherwise SPP.Zero.
        /// Only one +1 bonus per realm (not per matching card).
        /// </summary>
        public static SPP CalculateTerrainBonus(VehicleStack stack, RealmTrack realmTrack)
        {
            if (stack.HasFinished || stack.RealmIndex < 0)
                return SPP.Zero;

            var realmTerrain = realmTrack.GetTerrainAt(stack.RealmIndex);
            if (realmTerrain == TerrainIcon.None)
                return SPP.Zero;

            // Check if any card in the stack shares terrain with the realm
            foreach (var card in stack.AllCards())
            {
                if ((card.Data.TerrainIcons & realmTerrain) != TerrainIcon.None)
                {
                    return new SPP(1, 1, 1);
                }
            }

            return SPP.Zero;
        }
    }
}
