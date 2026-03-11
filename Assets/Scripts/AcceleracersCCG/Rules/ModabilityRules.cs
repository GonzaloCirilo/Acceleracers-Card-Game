using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Rules
{
    /// <summary>
    /// Rules for modability icon matching between Mods and Vehicles.
    /// </summary>
    public static class ModabilityRules
    {
        /// <summary>
        /// Checks if a mod can be equipped on a vehicle (at least 1 shared modability icon).
        /// </summary>
        public static bool CanEquipMod(CardData mod, CardData vehicle)
        {
            if (mod is not ModCardData modData)
                return false;
            if (vehicle is not VehicleCardData vehicleData)
                return false;

            return (modData.ModabilityIcons & vehicleData.ModabilityIcons) != ModabilityIcon.None;
        }

        /// <summary>
        /// Returns a validation message if the mod can't be equipped, or null if valid.
        /// </summary>
        public static string ValidateModEquip(CardData mod, CardData vehicle)
        {
            if (mod is not ModCardData modData)
                return "Card is not a Mod.";
            if (vehicle is not VehicleCardData vehicleData)
                return "Target is not a Vehicle.";

            if ((modData.ModabilityIcons & vehicleData.ModabilityIcons) == ModabilityIcon.None)
                return $"Mod modability ({modData.ModabilityIcons}) does not match " +
                       $"Vehicle modability ({vehicleData.ModabilityIcons}).";

            return null;
        }
    }
}
