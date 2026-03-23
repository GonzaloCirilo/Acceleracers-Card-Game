using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    /// <summary>
    /// Card types that carry SPP stats: Vehicle, Mod, Shift, AcceleCharger.
    /// Hazards and Racing Realms do not have their own SPP.
    /// </summary>
    public interface ISPPCard
    {
        SPP SPP { get; }
    }
}
