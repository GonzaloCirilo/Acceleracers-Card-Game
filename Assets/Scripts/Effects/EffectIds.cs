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
    }
}
