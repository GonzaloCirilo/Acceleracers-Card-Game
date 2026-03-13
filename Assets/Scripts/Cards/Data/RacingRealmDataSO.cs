using UnityEngine;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards.Data
{
    [CreateAssetMenu(fileName = "NewRacingRealm", menuName = "AcceleracersCCG/Racing Realm")]
    public class RacingRealmDataSO : CardDataSO
    {
        [Header("Racing Realm")]
        public int escapeValue;
        public SPPCategory escapeCategory;

        public override CardData ToCardData()
        {
            return new RacingRealmCardData(cardId, cardName, escapeValue, escapeCategory,
                terrainIcons, effectIds: effectIds);
        }
    }
}
