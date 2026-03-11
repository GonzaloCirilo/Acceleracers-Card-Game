using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects
{
    /// <summary>
    /// A card effect that produces commands when triggered.
    /// </summary>
    public interface ICardEffect
    {
        /// <summary>
        /// Resolve the effect and return commands to execute.
        /// </summary>
        List<ICommand> Resolve(GameState state, CardEffectContext context);
    }
}
