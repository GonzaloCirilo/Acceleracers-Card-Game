using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects.Implementations
{
    /// <summary>
    /// "Cannot be destroyed by Hazards" — passive effect that grants immunity.
    /// This is checked during Hazard targeting validation rather than producing commands.
    /// </summary>
    public class HazardImmunityEffect : ICardEffect
    {
        public List<ICommand> Resolve(GameState state, CardEffectContext context)
        {
            // Passive — hazard immunity is checked in targeting rules.
            return new List<ICommand>();
        }
    }
}
