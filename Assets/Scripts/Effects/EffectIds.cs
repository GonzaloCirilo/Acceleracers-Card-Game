namespace AcceleracersCCG.Effects
{
    /// <summary>
    /// String constants for card effect IDs, used for passive effect checks
    /// in rules and commands without going through EffectRegistry.
    /// </summary>
    public static class EffectIds
    {
        public const string HazardImmunity = "hazard_immunity";
        public const string IgnoreModability = "ignore_modability";
        public const string PersistOnAdvance = "persist_on_advance";
        public const string TimedDestruction = "timed_destruction";
        public const string ApplyCountdown = "apply_countdown";
        public const string BlockShift = "block_shift";
        public const string BlockMod = "block_mod";
        public const string BlockAcceleCharger = "block_accelecharger";
        public const string JunkAllRaceMods = "junk_all_race_mods";
        public const string RecoverModFromJunk = "recover_mod_from_junk";
        public const string RecoverModsForAP = "recover_mods_for_ap";
        public const string TransferMod = "transfer_mod";
        public const string TransferModIgnoreModability = "transfer_mod_ignore_modability";

        // Parameterized prefix — append ":N" where N is the increase amount.
        // e.g. "increase_hand_size:1" = hand limit is MaxHandSize + 1 while this vehicle is in play.
        public const string IncreaseHandSizePrefix = "increase_hand_size";

        // Parameterized prefix — append ":speed", ":power", or ":performance".
        // e.g. "recover_mod_with_spp:speed" = recover 1 mod from junk that has a Speed value.
        public const string RecoverModWithSPPPrefix = "recover_mod_with_spp";

        // Parameterized prefixes — append ":N" where N is 1-based realm number.
        // e.g. "retain_shifts_on_realm_advance:2" = retain shifts when advancing to 2nd realm.
        public const string RetainShiftsOnRealmAdvancePrefix = "retain_shifts_on_realm_advance";
        public const string RetainAcceleChargerOnRealmAdvancePrefix = "retain_accelecharger_on_realm_advance";

        /// <summary>
        /// Parses a parameterized effect ID like "prefix:value" and returns the int value.
        /// Returns -1 if the effect doesn't match the prefix.
        /// </summary>
        public static int ParseIntParam(string effectId, string prefix)
        {
            if (effectId != null && effectId.StartsWith(prefix + ":") &&
                int.TryParse(effectId.Substring(prefix.Length + 1), out int value))
                return value;
            return -1;
        }

        public static string ParseStringParam(string effectId, string prefix)
        {
            if (effectId != null && effectId.StartsWith(prefix + ":"))
                return effectId.Substring(prefix.Length + 1);
            return null;
        }
    }
}
