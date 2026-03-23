namespace AcceleracersCCG.Cards
{
    /// <summary>
    /// Marker for card types that have an Action Point cost to play: Mod, Shift, AcceleCharger, Hazard.
    /// Racing Realms and Vehicles never have an AP cost.
    /// </summary>
    public interface IAPCostCard
    {
        int APCost { get; }
    }
}
