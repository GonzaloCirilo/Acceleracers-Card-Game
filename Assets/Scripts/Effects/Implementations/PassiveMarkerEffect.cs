using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Effects.Implementations
{
    /// <summary>
    /// No-op placeholder for passive effects whose logic is enforced elsewhere in rules
    /// or phases (e.g. auto_advance_next_turn in AdvancePhase, retain_mods_on_advance in
    /// AdvanceRules). Registered so the EffectRegistry recognizes the id.
    /// </summary>
    public class PassiveMarkerEffect : ICardEffect
    {
        public List<ICommand> Resolve(GameState state, CardEffectContext context)
        {
            return new List<ICommand>();
        }
    }
}
