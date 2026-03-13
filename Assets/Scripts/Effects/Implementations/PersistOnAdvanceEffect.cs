using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects.Implementations
{
    /// <summary>
    /// Shift or AcceleCharger that survives realm transition (not stripped on advance).
    /// Checked during StripTemporaries processing.
    /// </summary>
    public class PersistOnAdvanceEffect : ICardEffect
    {
        public List<ICommand> Resolve(GameState state, CardEffectContext context)
        {
            // Passive — persistence is checked in strip temporaries logic.
            return new List<ICommand>();
        }
    }
}
