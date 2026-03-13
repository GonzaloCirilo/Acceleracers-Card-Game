using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;
using AcceleracersCCG.Effects;

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
        public static string ValidateAcceleCharger(CardData card, VehicleStack stack, RealmTrack realmTrack = null)
        {
            if (card.CardType != CardType.AcceleCharger)
                return "Card is not an AcceleCharger.";
            if (stack.AcceleCharger != null)
                return "Vehicle already has an AcceleCharger equipped.";

            if (realmTrack != null && !stack.HasFinished && stack.RealmIndex < Constants.RealmsPerRace)
            {
                var realm = realmTrack.GetRealm(stack.RealmIndex);
                if (realm?.Data.HasEffect(EffectIds.BlockAcceleCharger) == true)
                    return "AcceleChargers cannot be equipped to vehicles in this Realm.";
            }

            return null;
        }

        /// <summary>
        /// Validates equipping a Shift to a vehicle stack.
        /// </summary>
        public static string ValidateShift(CardData card, VehicleStack stack, RealmTrack realmTrack = null)
        {
            if (card.CardType != CardType.Shift)
                return "Card is not a Shift.";

            if (realmTrack != null && !stack.HasFinished && stack.RealmIndex < Constants.RealmsPerRace)
            {
                var realm = realmTrack.GetRealm(stack.RealmIndex);
                if (realm?.Data.HasEffect(EffectIds.BlockShift) == true)
                    return "Shifts cannot be equipped to vehicles in this Realm.";
            }

            return null;
        }

        /// <summary>
        /// Validates equipping a Mod to a vehicle stack (delegates modability check).
        /// </summary>
        public static string ValidateMod(CardData mod, VehicleStack stack, RealmTrack realmTrack = null)
        {
            if (mod.CardType != CardType.Mod)
                return "Card is not a Mod.";

            if (realmTrack != null && !stack.HasFinished && stack.RealmIndex < Constants.RealmsPerRace)
            {
                var realm = realmTrack.GetRealm(stack.RealmIndex);
                if (realm?.Data.HasEffect(EffectIds.BlockMod) == true)
                    return "Mods cannot be equipped to vehicles in this Realm.";
            }

            return ModabilityRules.ValidateModEquip(mod, stack.Vehicle.Data);
        }

        /// <summary>
        /// General equip validation — dispatches to the right validator by card type.
        /// </summary>
        public static string ValidateEquip(CardData card, VehicleStack stack, RealmTrack realmTrack = null)
        {
            return card.CardType switch
            {
                CardType.Mod => ValidateMod(card, stack, realmTrack),
                CardType.Shift => ValidateShift(card, stack, realmTrack),
                CardType.AcceleCharger => ValidateAcceleCharger(card, stack, realmTrack),
                _ => $"Card type {card.CardType} cannot be equipped."
            };
        }
    }
}
