using UnityEngine;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards.Data
{
    [CreateAssetMenu(fileName = "NewShift", menuName = "AcceleracersCCG/Shift")]
    public class ShiftDataSO : CardDataSO
    {
        public int apCost;

        public override CardData ToCardData()
        {
            return new ShiftCardData(cardId, cardName, new SPP(speed, power, performance),
                apCost: apCost, terrainIcons: terrainIcons, effectIds: effectIds);
        }
    }
}
