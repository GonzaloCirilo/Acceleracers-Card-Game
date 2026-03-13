using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public sealed class RacingRealmCardData : CardData
    {
        public override CardType CardType => CardType.RacingRealm;
        public int EscapeValue { get; }
        public SPPCategory EscapeCategory { get; }

        public RacingRealmCardData(string id, string name,
            int escapeValue, SPPCategory escapeCategory,
            TerrainIcon terrainIcons = TerrainIcon.None)
            : base(id, name, terrainIcons: terrainIcons)
        {
            EscapeValue = escapeValue;
            EscapeCategory = escapeCategory;
        }
    }
}
