using System.Collections.Generic;
using AcceleracersCCG.Core;
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
            registry.Register("recover_mods_for_ap", new RecoverModsForAPEffect());
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

            // Handle parameterized effects (e.g. "recover_mod_with_spp:speed")
            var resolved = ResolveParameterized(effectId);
            if (resolved != null) return resolved;

            throw new System.ArgumentException($"Unknown effect ID: '{effectId}'. Register it before use.");
        }

        private ICardEffect ResolveParameterized(string effectId)
        {
            var sppParam = EffectIds.ParseStringParam(effectId, EffectIds.RecoverModWithSPPPrefix);
            if (sppParam != null)
            {
                var category = RecoverModWithSPPEffect.ParseCategory(sppParam);
                var effect = new RecoverModWithSPPEffect(category);
                _effects[effectId] = effect; // cache for next lookup
                return effect;
            }

            var cardTypeParam = EffectIds.ParseStringParam(effectId, EffectIds.RecoverCardFromJunkPrefix);
            if (cardTypeParam != null)
            {
                var cardType = RecoverCardFromJunkEffect.ParseCardType(cardTypeParam);
                var effect = new RecoverCardFromJunkEffect(cardType);
                _effects[effectId] = effect;
                return effect;
            }

            return null;
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
