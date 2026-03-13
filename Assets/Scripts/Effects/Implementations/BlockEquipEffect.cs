using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects.Implementations
{
    /// <summary>
    /// Passive effect that blocks equipping certain card types to vehicles in this Realm.
    /// The actual logic is enforced in EquipRules; this is a no-op placeholder for the registry.
    /// </summary>
    public class BlockEquipEffect : ICardEffect
    {
        public List<ICommand> Resolve(GameState state, CardEffectContext context)
        {
            return new List<ICommand>();
        }
    }
}
