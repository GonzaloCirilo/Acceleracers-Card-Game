using System.Collections.Generic;
using AcceleracersCCG.Effects.Implementations;

namespace AcceleracersCCG.Effects
{
    /// <summary>
    /// Registry mapping effect IDs to their implementations.
    /// Populated at startup.
    /// </summary>
    public class EffectRegistry
    {
        private readonly Dictionary<string, ICardEffect> _effects = new Dictionary<string, ICardEffect>();

        public static EffectRegistry CreateDefault()
        {
            var registry = new EffectRegistry();
            registry.Register("null", new NullEffect());
            registry.Register("timed_destruction", new TimedDestructionEffect());
            registry.Register("hazard_immunity", new HazardImmunityEffect());
            registry.Register("ignore_modability", new IgnoreModabilityEffect());
            registry.Register("persist_on_advance", new PersistOnAdvanceEffect());
            return registry;
        }

        public void Register(string effectId, ICardEffect effect)
        {
            _effects[effectId] = effect;
        }

        public ICardEffect Get(string effectId)
        {
            if (string.IsNullOrEmpty(effectId))
                return _effects.GetValueOrDefault("null") ?? new NullEffect();

            return _effects.TryGetValue(effectId, out var effect)
                ? effect
                : new NullEffect();
        }

        public bool Has(string effectId) => _effects.ContainsKey(effectId);
    }
}
