using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;
using AcceleracersCCG.Effects;

namespace AcceleracersCCG.Rules
{
    /// <summary>
    /// Rules for Hazard card targeting.
    /// Hazards target opponent's Shifts or Mods (not AcceleChargers unless card says so).
    /// Blank SPP windows on the Hazard are ignored.
    /// </summary>
    public static class HazardTargetRules
    {
        /// <summary>
        /// Checks if a hazard can target a specific equipment card.
        /// </summary>
        public static bool CanTarget(CardData hazard, CardInstance target)
        {
            if (hazard is not HazardCardData hazardData)
                return false;

            if (target.Data.HasEffect(EffectIds.HazardImmunity))
                return false;

            var targetType = target.Data.CardType;

            if (targetType == CardType.AcceleCharger)
                return hazardData.CanTargetAcceleChargers;

            if (targetType == CardType.Vehicle)
                return hazardData.CanTargetVehicles;

            if (targetType != CardType.Shift && targetType != CardType.Mod)
                return false;

            return true;
        }

        /// <summary>
        /// Apply hazard damage to a target card's SPP.
        /// Blank windows (zero damage) on the hazard are ignored.
        /// Returns the resulting SPP after damage.
        /// </summary>
        public static SPP ApplyDamage(SPP targetSPP, SPP hazardDamage)
        {
            int speed = targetSPP.Speed;
            int power = targetSPP.Power;
            int perf = targetSPP.Performance;

            if (hazardDamage.Speed > 0)
                speed -= hazardDamage.Speed;
            if (hazardDamage.Power > 0)
                power -= hazardDamage.Power;
            if (hazardDamage.Performance > 0)
                perf -= hazardDamage.Performance;

            return new SPP(speed, power, perf);
        }

        /// <summary>
        /// Checks if the target should be junked after damage.
        /// A card is junked if any of its SPP stats drops to 0 or below.
        /// </summary>
        public static bool ShouldJunk(SPP resultSPP)
        {
            return resultSPP.AnyZeroOrBelow();
        }

        /// <summary>
        /// Returns validation message if hazard can't target the card, null if valid.
        /// </summary>
        public static string ValidateTarget(CardData hazard, CardInstance target)
        {
            if (hazard is not HazardCardData hazardData)
                return "Card is not a Hazard.";

            if (target.Data.HasEffect(EffectIds.HazardImmunity))
                return "Target is immune to Hazards.";

            var targetType = target.Data.CardType;

            if (targetType == CardType.AcceleCharger && !hazardData.CanTargetAcceleChargers)
                return "AcceleChargers are immune to this Hazard.";

            if (targetType == CardType.Vehicle && !hazardData.CanTargetVehicles)
                return "This Hazard cannot target Vehicles.";

            if (targetType != CardType.Shift && targetType != CardType.Mod
                && targetType != CardType.AcceleCharger && targetType != CardType.Vehicle)
                return $"Cannot target card type {targetType} with a Hazard.";

            return null;
        }
    }
}
