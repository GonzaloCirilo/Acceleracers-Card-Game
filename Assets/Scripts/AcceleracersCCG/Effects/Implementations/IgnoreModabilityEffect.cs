using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects.Implementations
{
    /// <summary>
    /// "Modability rules DO NOT apply" — passive effect.
    /// Checked during equip validation to bypass modability icon matching.
    /// </summary>
    public class IgnoreModabilityEffect : ICardEffect
    {
        public List<ICommand> Resolve(GameState state, CardEffectContext context)
        {
            // Passive — modability bypass is checked in equip rules.
            return new List<ICommand>();
        }
    }
}
