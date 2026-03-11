using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Rules
{
    /// <summary>
    /// Rules for vehicle advancement through realms.
    /// </summary>
    public static class AdvanceRules
    {
        /// <summary>
        /// Checks if a vehicle stack can advance past its current realm.
        /// The stack's total SPP in the realm's escape category must meet or exceed the escape value.
        /// </summary>
        public static bool CanAdvance(VehicleStack stack, RealmTrack realmTrack)
        {
            if (stack.HasFinished)
                return false;

            var realm = realmTrack.GetRealm(stack.RealmIndex);
            if (realm?.Data is not RacingRealmCardData realmData)
                return false;

            var totalSPP = SPPCalculator.Calculate(stack, realmTrack);
            return totalSPP.MeetsEscape(realmData.EscapeValue, realmData.EscapeCategory);
        }

        /// <summary>
        /// Returns a validation message if the vehicle cannot advance, or null if it can.
        /// </summary>
        public static string ValidateAdvance(VehicleStack stack, RealmTrack realmTrack)
        {
            if (stack.HasFinished)
                return "Vehicle has already finished the race.";

            var realm = realmTrack.GetRealm(stack.RealmIndex);
            if (realm?.Data is not RacingRealmCardData realmData)
                return "No realm at current position.";

            var totalSPP = SPPCalculator.Calculate(stack, realmTrack);
            var escapeCategory = realmData.EscapeCategory;
            var escapeValue = realmData.EscapeValue;

            if (!totalSPP.MeetsEscape(escapeValue, escapeCategory))
                return $"Vehicle {escapeCategory} ({totalSPP.GetCategory(escapeCategory)}) " +
                       $"does not meet escape value ({escapeValue}).";

            return null;
        }
    }
}
