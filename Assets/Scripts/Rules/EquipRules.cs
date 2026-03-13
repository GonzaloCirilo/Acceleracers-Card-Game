using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Rules
{
    /// <summary>
    /// General equipment validation rules.
    /// </summary>
    public static class EquipRules
    {
        /// <summary>
        /// Validates equipping an AcceleCharger to a vehicle stack.
        /// Max 1 AcceleCharger per vehicle.
        /// </summary>
        public static string ValidateAcceleCharger(CardData card, VehicleStack stack)
        {
            if (card.CardType != CardType.AcceleCharger)
                return "Card is not an AcceleCharger.";
            if (stack.AcceleCharger != null)
                return "Vehicle already has an AcceleCharger equipped.";
            return null;
        }

        /// <summary>
        /// Validates equipping a Shift to a vehicle stack.
        /// </summary>
        public static string ValidateShift(CardData card, VehicleStack stack)
        {
            if (card.CardType != CardType.Shift)
                return "Card is not a Shift.";
            return null;
        }

        /// <summary>
        /// Validates equipping a Mod to a vehicle stack (delegates modability check).
        /// </summary>
        public static string ValidateMod(CardData mod, VehicleStack stack)
        {
            if (mod.CardType != CardType.Mod)
                return "Card is not a Mod.";

            return ModabilityRules.ValidateModEquip(mod, stack.Vehicle.Data);
        }

        /// <summary>
        /// General equip validation — dispatches to the right validator by card type.
        /// </summary>
        public static string ValidateEquip(CardData card, VehicleStack stack)
        {
            return card.CardType switch
            {
                CardType.Mod => ValidateMod(card, stack),
                CardType.Shift => ValidateShift(card, stack),
                CardType.AcceleCharger => ValidateAcceleCharger(card, stack),
                _ => $"Card type {card.CardType} cannot be equipped."
            };
        }
    }
}
