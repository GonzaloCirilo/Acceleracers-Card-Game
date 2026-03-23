using UnityEngine;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards.Data
{
    /// <summary>
    /// Abstract ScriptableObject base for card definitions.
    /// Used for Unity Editor authoring. Each subclass converts to the matching CardData subclass.
    /// </summary>
    public abstract class CardDataSO : ScriptableObject
    {
        [Header("Base Card Data")]
        public string cardId;
        public string cardName;
        [TextArea] public string cardText;
        public string[] effectIds;

        [Header("Terrain")]
        public TerrainIcon terrainIcons;

        /// <summary>
        /// Convert this SO into a runtime CardData subclass.
        /// </summary>
        public abstract CardData ToCardData();
    }
}
