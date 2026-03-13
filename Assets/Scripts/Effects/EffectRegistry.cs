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
            registry.Register("block_shift", new BlockEquipEffect());
            registry.Register("block_mod", new BlockEquipEffect());
            registry.Register("block_accelecharger", new BlockEquipEffect());
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

            if (_effects.TryGetValue(effectId, out var effect))
                return effect;

            throw new System.ArgumentException($"Unknown effect ID: '{effectId}'. Register it before use.");
        }

        public List<ICardEffect> GetAll(IEnumerable<string> effectIds)
        {
            var results = new List<ICardEffect>();
            foreach (var id in effectIds)
            {
                results.Add(Get(id));
            }
            return results;
        }

        public bool Has(string effectId) => _effects.ContainsKey(effectId);
    }
}
