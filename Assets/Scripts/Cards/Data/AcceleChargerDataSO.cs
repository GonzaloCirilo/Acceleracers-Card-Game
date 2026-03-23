using UnityEngine;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards.Data
{
    [CreateAssetMenu(fileName = "NewAcceleCharger", menuName = "AcceleracersCCG/AcceleCharger")]
    public class AcceleChargerDataSO : CardDataSO
    {
        [Header("SPP Stats")]
        public int speed;
        public int power;
        public int performance;

        [Header("AcceleCharger")]
        public int apCost;
        public string acceleronSymbol;

        public override CardData ToCardData()
        {
            return new AcceleChargerCardData(cardId, cardName, new SPP(speed, power, performance),
                apCost: apCost, terrainIcons: terrainIcons,
                acceleronSymbol: acceleronSymbol, effectIds: effectIds);
        }
    }
}
