using UnityEngine;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards.Data
{
    [CreateAssetMenu(fileName = "NewMod", menuName = "AcceleracersCCG/Mod")]
    public class ModDataSO : CardDataSO
    {
        [Header("SPP Stats")]
        public int speed;
        public int power;
        public int performance;

        [Header("Mod")]
        public int apCost;
        public ModabilityIcon modabilityIcons;

        public override CardData ToCardData()
        {
            return new ModCardData(cardId, cardName, new SPP(speed, power, performance),
                apCost: apCost, modabilityIcons: modabilityIcons,
                terrainIcons: terrainIcons, effectIds: effectIds);
        }
    }
}
