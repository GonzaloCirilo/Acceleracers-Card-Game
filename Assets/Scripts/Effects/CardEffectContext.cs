using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;

namespace AcceleracersCCG.Effects
{
    /// <summary>
    /// Context information passed to card effects.
    /// </summary>
    public class CardEffectContext
    {
        /// <summary>Player index who owns the effect source.</summary>
        public int OwnerPlayerIndex { get; set; }

        /// <summary>The card that has the effect.</summary>
        public CardInstance SourceCard { get; set; }

        /// <summary>The vehicle stack the source card is on.</summary>
        public VehicleStack SourceStack { get; set; }

        /// <summary>Target card (for targeted effects).</summary>
        public CardInstance TargetCard { get; set; }

        /// <summary>Target vehicle stack (for targeted effects).</summary>
        public VehicleStack TargetStack { get; set; }

        /// <summary>What triggered this effect.</summary>
        public EffectTrigger Trigger { get; set; }
    }
}
